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

        public Task<ResponseService<List<LikeOutput>>> GetCommentLiks(int CommentId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseService<List<LikeOutput>>> GetPostLiks(int CommentId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseService<List<LikeOutput>>> GetSubCommentLiks(int CommentId)
        {
            throw new NotImplementedException();
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

        public async Task<ResponseService<bool>> IsSubCommentliked(int id, User user) =>
        new()
        {
            Data = await base.GetQuery().Where(s => (s as SubCommentLike).SubCommentID == id && s.UserId.Equals(user.Id)).AnyAsync(),
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

        public async Task<ResponseService<LikeOutput>> LikeSubComment(int id, User user)
        {
            var result = new ResponseService<LikeOutput>();
            try
            {

                if ((await dbContext.SubComments.FindAsync(id)) == null)
                {
                    return result.SetMessage("SubComment not found").SetStatus(StatusCodes.NotFound.ToString());
                }
                if ((await IsSubCommentliked(id, user)).Data)
                {
                    return result.SetMessage("You are already like this SubComment").SetStatus(StatusCodes.BadRequest.ToString());
                }
                else
                {
                    SubCommentLike like = new() { SubCommentID = id, LikeDate = DateTime.Now, UserId = user.Id };
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

        public async Task<ResponseService<bool>> UnLikeSubComment(int id, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {
                var like = await base.GetQuery().Where(i => i.UserId.Equals(user.Id) && (i as SubCommentLike).SubCommentID == id).FirstOrDefaultAsync();
                if (like == null)
                    return result.SetMessage("You are  Not Liked this Comment").SetStatus(StatusCodes.NotFound.ToString());
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
        public Task<ResponseService<LikeOutput>> LikeSubComment(int SubCommentId, User user);
        public Task<ResponseService<bool>> UnLikeComment(int CommentId, User user);
        public Task<ResponseService<bool>> UnLikePost(int PostId, User user);
        public Task<ResponseService<bool>> UnLikeSubComment(int SubCommentId, User user);
        public Task<ResponseService<bool>> Unlike(int LikIid, User user);
        public Task<ResponseService<bool>> IsCommentliked(int CommentId, User user);
        public Task<ResponseService<bool>> IsPostliked(int PostId, User user);
        public Task<ResponseService<bool>> IsSubCommentliked(int SubCommentId, User user);

        public Task<ResponseService<List<LikeOutput>>> GetCommentLiks(int CommentId);
        public Task<ResponseService<List<LikeOutput>>> GetPostLiks(int CommentId);
        public Task<ResponseService<List<LikeOutput>>> GetSubCommentLiks(int CommentId);
    }
}