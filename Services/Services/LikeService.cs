using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Helper;
using Models.Like.Output;
using Services.Common;

namespace Services.Services
{
    public class LikeService : GenericRepository<Like>, ILikeService
    {
        StoreContext dbContext;
        public LikeService(StoreContext dbContext, IMapper _mapper) : base(dbContext, _mapper)
        {
            this.dbContext = dbContext;
        }

        public async Task<PagedList<LikeOutput>> GetCommentLiks(DynamicPagination input, int CommentId) =>
            _mapper.Map<PagedList<Like>, PagedList<LikeOutput>>(await PagedList<Like>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Where(i => (i as CommentLike).CommentID == CommentId).OrderByDescending(i => i.LikeDate), input));

        public async Task<ResponseService<long>> getCommentTotalLike(int CommentId)
        {
            if (!await dbContext.Comments.Where(s => s.Id == CommentId).AnyAsync())
                return new ResponseService<long>() { Message = "Comment Notfound", Status = StatusCodes.NotFound.ToString() };
            var result = await base.GetQuery().Where(s => (s as CommentLike).CommentID == CommentId).LongCountAsync();
            return new ResponseService<long>() { Data = result, Message = "ok", Status = StatusCodes.Ok.ToString() };

        }

        public async Task<PagedList<LikeOutput>> GetMyLikeHistory(Pagination input, User user)
        {
            var Query = base.GetQuery().Include(s => s.User).Where(s => s.User.NormalizedUserName.Equals(user.UserName.ToUpper())).Include(s => (s as CommentLike).Comment).Include(s => (s as PostLike).Post).OrderByDescending(i => i.LikeDate);
            return _mapper.Map<PagedList<Like>, PagedList<LikeOutput>>(await PagedList<Like>.CreatePagedListAsync(Query,input));
        }

        public async Task<PagedList<LikeOutput>> GetPostLiks(DynamicPagination input, int PostId) =>
            _mapper.Map<PagedList<Like>, PagedList<LikeOutput>>(await PagedList<Like>.CreatePagedListAsync(base.GetQuery().Include(i => i.User).Where(i => (i as PostLike).PostID == PostId).OrderByDescending(i => i.LikeDate), input));

        public async Task<ResponseService<long>> getPostTotalLike(int PostId)
        {
            if (!await dbContext.Posts.Where(s => s.Id == PostId).AnyAsync())
                return new ResponseService<long>() { Message = "Post Notfound", Status = StatusCodes.NotFound.ToString() };

            var result = await base.GetQuery().Where(s => (s as PostLike).PostID == PostId).LongCountAsync();
            return new ResponseService<long>() { Data = result, Message = "ok", Status = StatusCodes.Ok.ToString() };
        }

        public async Task<ResponseService<bool>> IsCommentliked(int id, User user) =>
             new()
             {
                 Data = await base.GetQuery().Where(s => (s as CommentLike).CommentID == id && s.UserId.Equals(user.Id)).AnyAsync(),
                 Message = "Ok",
                 Status = StatusCodes.Ok.ToString()
             };


        public async Task<ResponseService<bool>> IsPostliked(int id, User user) =>
         new()
         {
             Data = await base.GetQuery().Where(s => (s as PostLike).PostID == id && s.UserId.Equals(user.Id)).AnyAsync(),
             Message = "Ok",
             Status = StatusCodes.Ok.ToString()
         };


        public async Task<ResponseService<LikeOutput>> LikeComment(int id, User user)
        {
            var result = new ResponseService<LikeOutput>();
            try
            {

                if ((await dbContext.Comments.FindAsync(id)) == null)
                {
                    return result.SetMessage("Comment not found").SetStatus(StatusCodes.NotFound.ToString());
                }
                if ((await IsCommentliked(id, user)).Data)
                {
                    return result.SetMessage("You are already like this Comment").SetStatus(StatusCodes.BadRequest.ToString());
                }
                else
                {
                    CommentLike like = new() { CommentID = id, LikeDate = DateTime.Now, UserId = user.Id };
                    await base.InsertAsync(like);
                    if (await base.CompleteAsync())
                    {
                        like.User = user;
                        return result.SetData(_mapper.Map<Like, LikeOutput>(like)).SetMessage("Liked").SetStatus(StatusCodes.Ok.ToString());
                    }
                    else
                    {
                        return result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
                    }
                }
            }
            catch
            {
                return ResponseService<LikeOutput>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<LikeOutput>> LikePost(int id, User user)
        {
            var result = new ResponseService<LikeOutput>();
            try
            {

                if ((await dbContext.Posts.FindAsync(id)) == null)
                {
                    return result.SetMessage("Post not found").SetStatus(StatusCodes.NotFound.ToString());
                }
                if ((await IsPostliked(id, user)).Data)
                {
                    return result.SetMessage("You are already like this Post").SetStatus(StatusCodes.BadRequest.ToString());
                }
                else
                {
                    PostLike like = new() { PostID = id, LikeDate = DateTime.Now, UserId = user.Id };
                    await base.InsertAsync(like);
                    if (await base.CompleteAsync())
                    {
                        like.User = user;
                        return result.SetData(_mapper.Map<Like, LikeOutput>(like)).SetMessage("Liked").SetStatus(StatusCodes.Ok.ToString());
                    }
                    else
                    {
                        return result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
                    }
                }
            }
            catch
            {
                return ResponseService<LikeOutput>.GetExeptionResponse();
            }
        }



        public async Task<ResponseService<bool>> Unlike(int LikIid, User user)
        {

            var result = new ResponseService<bool>() { Data = false };
            try
            {
                var like = await base.GetByIdAsync(LikIid);
                if (like == null)
                    return result.SetMessage("like Not Found").SetStatus(StatusCodes.NotFound.ToString());

                if (!like.UserId.Equals(user.Id))
                    return result.SetMessage("like not owned by you").SetStatus(StatusCodes.Forbidden.ToString());

                await base.DeleteAsync(like.Id);
                return await base.CompleteAsync() ?
                     result.SetData(true).SetMessage("UnLiked").SetStatus(StatusCodes.Ok.ToString())
                    :
                     result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> UnLikeComment(int id, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {
                var like = await base.GetQuery().Where(i => i.UserId.Equals(user.Id) && (i as CommentLike).CommentID == id).FirstOrDefaultAsync();
                if (like == null)
                    return result.SetMessage("You are  Not Liked this comment").SetStatus(StatusCodes.NotFound.ToString());
                await base.DeleteAsync(like.Id);
                return await base.CompleteAsync() ?
                     result.SetData(true).SetMessage("UnLiked").SetStatus(StatusCodes.Ok.ToString())
                    :
                     result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> UnLikePost(int id, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {
                var like = await base.GetQuery().Where(i => i.UserId.Equals(user.Id) && (i as PostLike).PostID == id).FirstOrDefaultAsync();
                if (like == null)
                    return result.SetMessage("You are  Not Liked this Post").SetStatus(StatusCodes.NotFound.ToString());
                await base.DeleteAsync(like.Id);
                return await base.CompleteAsync() ?
                     result.SetData(true).SetMessage("UnLiked").SetStatus(StatusCodes.Ok.ToString())
                    :
                     result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }
    }
    public interface ILikeService
    {
        public Task<ResponseService<LikeOutput>> LikeComment(int CommentId, User user);
        public Task<ResponseService<LikeOutput>> LikePost(int PostId, User user);
        public Task<ResponseService<long>> getPostTotalLike(int PostId);
        public Task<ResponseService<long>> getCommentTotalLike(int CommentId);
        public Task<ResponseService<bool>> UnLikeComment(int CommentId, User user);
        public Task<ResponseService<bool>> UnLikePost(int PostId, User user);
        public Task<ResponseService<bool>> Unlike(int LikIid, User user);
        public Task<ResponseService<bool>> IsCommentliked(int CommentId, User user);
        public Task<ResponseService<bool>> IsPostliked(int PostId, User user);
        public Task<PagedList<LikeOutput>> GetCommentLiks(DynamicPagination input, int CommentId);
        public Task<PagedList<LikeOutput>> GetMyLikeHistory(Pagination input, User user);
        public Task<PagedList<LikeOutput>> GetPostLiks(DynamicPagination input, int PostId);
    }
}
