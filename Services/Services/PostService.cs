using System.Net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Post.Input;
using Models.Post.Output;
using Services.Common;
using Microsoft.EntityFrameworkCore;

namespace Services.Services
{
    public class PostService : GenericRepository<Post>, IPostService
    {
        private readonly IMapper _mapper;
        IGenericRepository<Tag> _tags;
        StoreContext dbContext;
        public PostService(IGenericRepository<Tag> tags, StoreContext dbContext, IMapper _mapper) : base(dbContext)
        {
            this.dbContext = dbContext;
            this._mapper = _mapper;
            this._tags = tags;
        }

        public async Task<ResponseService<PostOutput>> Create(PostCreateInput input, User user)
        {
            var result = new ResponseService<PostOutput>();

            var Transaction = await base.BeginTransactionAsync();
            try
            {
                var post = _mapper.Map<PostCreateInput, Post>(input);
                input.TagsID = input.TagsID != null && input.TagsID.Any() ? input.TagsID.Distinct().ToList() : null;
                post.UserId = user.Id;
                post.IsEdited = false;
                if (input.TagsID != null)
                {
                    var tagslist = await _tags.GetQuery().Where(s => input.TagsID.Contains(s.Id)).ToListAsync();
                    if (tagslist.Count != input.TagsID.Count)
                    {
                        Transaction.Rollback();
                        return result.SetStatus(StatusCodes.BadRequest.ToString()).SetMessage("some tag not found");
                    }
                    else
                    {
                        post.Tags = new List<PostTag>();
                        foreach (var item in tagslist)
                            post.Tags.Add(new PostTag { Tag = item, TagId = item.Id });
                    }
                }
                await base.InsertAsync(post);
                if (await base.CompleteAsync())
                {
                    Transaction.Commit();
                    post.User = user;
                    return result.SetData(_mapper.Map<Post, PostOutput>(post)).SetStatus(StatusCodes.Ok.ToString());
                }
                else
                {
                    await Transaction.RollbackAsync();
                    return result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
                }
            }
            catch
            {
                await Transaction.RollbackAsync();
                return ResponseService<PostOutput>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> Delete(int id, User user)
        {
            try
            {

                var result = new ResponseService<bool>() { Data = false };
                var post = await base.GetQuery().Where(s => s.Id == id).FirstOrDefaultAsync();
                if (post == default)
                {
                    return result.SetStatus(StatusCodes.NotFound.ToString());
                }
                if (post.UserId != user.Id)
                {
                    return result.SetStatus(StatusCodes.Unauthorized.ToString());
                }
                await base.DeleteAsync(id);
                return await base.CompleteAsync() ? result.SetData(true).SetStatus(StatusCodes.Ok.ToString())
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<PostOutput>> GetPost(int Id)
        {
            var post = await base.GetQuery().Include(i => i.User).Include(i => i.Tags).ThenInclude(s => s.Tag).Where(i => i.Id == Id).FirstOrDefaultAsync();
            var result = new ResponseService<PostOutput>
            {
                Data = _mapper.Map<Post, PostOutput>(post)
            };
            return result.Data != null ? result.SetMessage("ok").SetStatus(StatusCodes.Ok.ToString())
            :
            result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
        }

        public Task<ResponseService<List<PostOutput>>> GetUserHomePagePosts(User user)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseService<List<PostOutput>>> GetUserProfilePosts(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseService<PostOutput>> Update(PostUpdateInput input, User user)
        {
            var result = new ResponseService<PostOutput>();
            var post = await base.GetQuery().Include(i => i.Tags).ThenInclude(i => i.Tag).Include(i => i.User).Where(i => i.Id == input.Id).FirstOrDefaultAsync();
            if (!user.Id.Equals(post.UserId))
            {
                return result.SetStatus(StatusCodes.Unauthorized.ToString());
            }
            var Transaction = await base.BeginTransactionAsync();
            try
            {
                post.PostText = input.PostText;
                post.Type = input.Type;
                post.IsEdited = true;
                input.TagsID = input.TagsID != null && input.TagsID.Any() ? input.TagsID.Distinct().ToList() : null;
                if (input.TagsID != null)
                {
                    foreach (var item in input.TagsID.Where(i => !post.Tags.Where(x => x.TagId == i).Any()).ToList())
                    {
                        dbContext.PostTags.Add(new PostTag { TagId = item, PostId = post.Id });
                    }
                    var fordeltetag = post.Tags.Where(i => !input.TagsID.Where(x => x == i.TagId).Any()).ToList();
                    dbContext.PostTags.RemoveRange(fordeltetag);
                }
                else if (post.Tags.Any())
                {
                    dbContext.PostTags.RemoveRange(post.Tags);
                }
                base.Update(post);
                if (await base.CompleteAsync())
                {
                    Transaction.Commit();
                    var r = await base.GetQuery().Include(i => i.User).Include(i => i.Tags).Where(i => i.Id == post.Id).FirstOrDefaultAsync();
                    return result.SetData(_mapper.Map<Post, PostOutput>(r)).SetStatus(StatusCodes.Ok.ToString());
                }
                else
                {
                    await Transaction.RollbackAsync();
                    return result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
                }
            }
            catch
            {
                await Transaction.RollbackAsync();
                return ResponseService<PostOutput>.GetExeptionResponse();
            }
        }
    }
    public interface IPostService
    {
        public Task<ResponseService<PostOutput>> Create(PostCreateInput input, User user);
        public Task<ResponseService<PostOutput>> Update(PostUpdateInput input, User user);
        public Task<ResponseService<bool>> Delete(int id, User user);
        public Task<ResponseService<PostOutput>> GetPost(int Id);
        public Task<ResponseService<List<PostOutput>>> GetUserProfilePosts(User user);
        public Task<ResponseService<List<PostOutput>>> GetUserHomePagePosts(User user);
    }
}
