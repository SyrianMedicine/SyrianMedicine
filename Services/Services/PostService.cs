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
using Models.Helper;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
namespace Services.Services
{
    public class PostService : GenericRepository<Post>, IPostService
    {
        private readonly IGenericRepository<Tag> _tags;
        private readonly StoreContext dbContext;
        public PostService(IGenericRepository<Tag> tags, StoreContext dbContext, IMapper _mapper) : base(dbContext, _mapper)
        {
            this.dbContext = dbContext;
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
                        return result.SetStatus(StatusCodes.BadRequest.ToString()).SetMessage("Some tags not found");
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
                    var savedfile = true;
                    try
                    {
                        if (input.Media != null)
                        {
                            savedfile = await SaveFile(input.Media, post);
                            if (savedfile)
                            {
                                base.Update(post);
                                savedfile = await base.CompleteAsync();
                            }
                        }
                    }
                    catch { }

                    Transaction.Commit();
                    post.User = user;
                    result.SetData(_mapper.Map<Post, PostOutput>(post)).SetStatus(StatusCodes.Ok.ToString());
                    return savedfile ? result.SetMessage("post Added")
                    :
                    result.SetMessage("Post added but media not saved, please try adding media again");
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
        private async Task<bool> SaveFile(Microsoft.AspNetCore.Http.IFormFile file, Post post)
        {
            try
            {

                var postpath = "wwwroot/postMedia/" + post.User.NormalizedUserName + "/";
                if (!Directory.Exists(postpath))
                {
                    Directory.CreateDirectory(postpath);
                }
                var path = Path.Combine(postpath, "post" + post.Id + "_" + file.FileName);
                var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
                await stream.DisposeAsync();
                post.MedialUrl = path[7..];
                return true;
            }
            catch { return false; }
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

        public async Task<PagedList<PostOutput>> GetTagPosts(DynamicPagination input, int Tagid) =>
            _mapper.Map<PagedList<Post>, PagedList<PostOutput>>(await PagedList<Post>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Include(i => i.Tags).Where(i => i.Tags.Where(x => x.TagId == Tagid).Any()).OrderByDescending(i => i.Date), input));

        public async Task<PagedList<MostPostsRated>> GetTopPostsForThisMounth(Pagination input) =>
        _mapper.Map<PagedList<Post>, PagedList<MostPostsRated>>(await PagedList<Post>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Where(i => i.Date.Year == DateTime.UtcNow.Year && i.Date.Month == DateTime.UtcNow.Month).OrderByDescending(i => i.LikedByList.Count), input));

        public async Task<PagedList<PostOutput>> GetTopPostsForThisYear(Pagination input) =>
        _mapper.Map<PagedList<Post>, PagedList<PostOutput>>(await PagedList<Post>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Include(i => i.Tags).ThenInclude(s => s.Tag).Where(i => i.Date.Year == DateTime.UtcNow.Year).OrderByDescending(i => i.LikedByList.Count()), input));
        public async Task<PagedList<PostOutput>> GetUserHomePagePosts(DynamicPagination input, User user)
        {
            var usertagsIds = dbContext.UserTags.Where(i => i.UserId.Equals(user.Id));
            var Post = base.GetQuery().Where(x => usertagsIds.LeftJoin(x.Tags, i => i.TagId, i => i.TagId, (i, j) => i.TagId == j.TagId).Where(i => i).Any() || x.User.Followers.Where(c => c.UserId.Equals(user.Id)).Any());
            return _mapper.Map<PagedList<Post>, PagedList<PostOutput>>(await PagedList<Post>.CreatePagedListAsync(Post.Include(i => i.User).Include(i => i.Tags).ThenInclude(s => s.Tag).OrderByDescending(i => i.Date), input));
        }

        public async Task<PagedList<PostOutput>> GetUserProfilePosts(DynamicPagination input, string username) =>
            _mapper.Map<PagedList<Post>, PagedList<PostOutput>>(await PagedList<Post>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Include(i => i.Tags).ThenInclude(s => s.Tag).Where(i => i.User.NormalizedUserName.Equals(username.ToUpper())).OrderByDescending(i => i.Date), input));

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
        public async Task<PagedList<PostOutput>> GetPagedPosts(PostQuery input)
        {
            var query = base.GetQuery().Include(i => i.User).Include(i => i.Tags).ThenInclude(s => s.Tag).OrderByDescending(e => e.Date).AsQueryable();

            if (!String.IsNullOrEmpty(input.SearchString))
            {
                query = query.Where(e =>
                     e.PostText.ToLower().Contains(input.SearchString.ToLower()) ||
                     e.PostTitle.ToLower().Contains(input.SearchString.ToLower()) ||
                     e.Date.ToString().ToLower().Contains(input.SearchString.ToLower()) ||
                     e.User.FirstName.ToLower().Contains(input.SearchString.ToLower()) ||
                     e.User.LastName.ToLower().Contains(input.SearchString.ToLower()) ||
                     e.User.UserName.ToLower().Contains(input.SearchString.ToLower()));
            }
            if (!String.IsNullOrEmpty(input.TagName))
            {
                query = query.Where(e => e.Tags.Any(t => t.Tag.Tagname.ToLower().Equals(input.TagName.ToLower())));
            }

            return _mapper.Map<PagedList<Post>, PagedList<PostOutput>>(await PagedList<Post>.CreatePagedListAsync(query, input));
        }
    }
    public interface IPostService
    {
        public Task<ResponseService<PostOutput>> Create(PostCreateInput input, User user);
        public Task<ResponseService<PostOutput>> Update(PostUpdateInput input, User user);
        public Task<ResponseService<bool>> Delete(int id, User user);
        public Task<ResponseService<PostOutput>> GetPost(int Id);
        public Task<PagedList<PostOutput>> GetTagPosts(DynamicPagination input, int Tagid);
        public Task<PagedList<PostOutput>> GetUserProfilePosts(DynamicPagination input, string username);
        public Task<PagedList<PostOutput>> GetUserHomePagePosts(DynamicPagination input, User user);
        public Task<PagedList<MostPostsRated>> GetTopPostsForThisMounth(Pagination input);
        public Task<PagedList<PostOutput>> GetTopPostsForThisYear(Pagination input);
        public Task<PagedList<PostOutput>> GetPagedPosts(PostQuery input);
    }
}
