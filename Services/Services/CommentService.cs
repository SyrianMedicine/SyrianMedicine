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
using Models.Comment.Input;
using Models.Comment.Output;
using Models.Helper;
using Services.Common;

namespace Services.Services
{
    public class CommentService : GenericRepository<Comment>, ICommentService
    {
        private readonly IIdentityRepository identityRepository;
        private readonly IGenericRepository<Post> PostRepository;
        public CommentService(IGenericRepository<Post> PostRepository, IIdentityRepository identityRepository, IMapper _mapper, StoreContext dbContext) : base(dbContext, _mapper)
        {
            this.PostRepository = PostRepository;
            this.identityRepository = identityRepository;
        }
        public async Task<ResponseService<CommentOutput>> CreateSubComment(SubCommentCreateInput input, User user)
        {
            try
            {
                var result = new ResponseService<CommentOutput>();
                var Comment = _mapper.Map<SubCommentCreateInput, SubComment>(input);
                var baseComment = await base.GetQuery().Where(i => i.Id == input.CommentId).Include(s => s.User).FirstOrDefaultAsync();
                if (baseComment == default) return result.SetMessage("Comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                // todo: this user maybe blocked by post or Comment Owner >_<
                Comment.UserId = user.Id;
                Comment.Comment = baseComment;
                await base.InsertAsync(Comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<SubComment, CommentOutput>(Comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<CommentOutput>.GetExeptionResponse();
            }
        }
        public async Task<ResponseService<CommentOutput>> CreateAccountComment(AccountCommentCreateInput input, User user)
        {
            try
            {

                var result = new ResponseService<CommentOutput>();
                var Comment = _mapper.Map<AccountCommentCreateInput, AccountComment>(input);
                var Account = await identityRepository.GetUserByNameAsync(input.AccountUserName);
                if (Account == null) return result.SetMessage("Account " + input.AccountUserName + " not Found").SetStatus(StatusCodes.NotFound.ToString());

                // todo: this user maybe blocked by Account >_<

                Comment.OnAccountId = Account.Id;
                Comment.UserId = user.Id;
                Comment.OnAccount = Account;
                await base.InsertAsync(Comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<AccountComment, CommentOutput>(Comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<CommentOutput>.GetExeptionResponse();
            }


        }

        public async Task<ResponseService<CommentOutput>> CreatePostComment(PostCommentCreateInput input, User user)
        {
            try
            {
                var result = new ResponseService<CommentOutput>();
                var Comment = _mapper.Map<PostCommentCreateInput, PostComment>(input);
                var post = await PostRepository.GetByIdAsync(input.Postid);
                if (post == default) return result.SetMessage("Post not Found").SetStatus(StatusCodes.NotFound.ToString());

                // todo: this user maybe blocked by post Owner >_<

                Comment.UserId = user.Id;
                await base.InsertAsync(Comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<PostComment, CommentOutput>(Comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<CommentOutput>.GetExeptionResponse();
            }

        }

        public async Task<ResponseService<CommentOutput>> Update(CommentUpdateInput Input, User user)
        {
            try
            {

                var result = new ResponseService<CommentOutput>();
                var comment = await base.GetQuery().Include(s => s.User).Where(s => s.Id == Input.Id).Include(s => (s as AccountComment).OnAccount).Include(s => (s as PostComment).Post).FirstOrDefaultAsync();
                if (comment == default) return result.SetMessage("comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (!comment.UserId.Equals(user.Id)) return result.SetMessage("You are not the comment Owner").SetStatus(StatusCodes.Unauthorized.ToString());
                comment.IsEdited = true;
                comment.CommentText = Input.CommentText;
                base.Update(comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<Comment, CommentOutput>(comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<CommentOutput>.GetExeptionResponse();
            }
        }
        public async Task<ResponseService<bool>> delete(int id, User user)
        {
            try
            {
                var result = new ResponseService<bool>();
                var comment = await base.GetByIdAsync(id);
                if (comment == default) return result.SetMessage("comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (!comment.UserId.Equals(user.Id)) return result.SetMessage("You are not the comment Owner").SetStatus(StatusCodes.Unauthorized.ToString());
                await base.DeleteAsync(comment.Id);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(true).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());

            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<CommentOutput>> GetComment(int id)
        {
            var result = new ResponseService<CommentOutput>();
            var comment = await base.GetQuery().Include(s => s.User).Where(s => s.Id == id).Include(s => (s as AccountComment).OnAccount).Include(s => (s as SubComment).Comment).Include(s => (s as PostComment).Post).FirstOrDefaultAsync();
            return comment != null ? result.SetData(_mapper.Map<Comment, CommentOutput>(comment)).SetMessage("this comment").SetStatus(StatusCodes.Ok.ToString())
            :
            result.SetStatus(StatusCodes.NotFound.ToString()).SetMessage("comment not found");
        }

        public async Task<PagedList<CommentOutput>> GetMyComments(Pagination input, User User)
        {
            var Query = base.GetQuery().Include(s => s.User).Where(s => s.User.NormalizedUserName.Equals(User.UserName.ToUpper())).Include(s => (s as AccountComment).OnAccount).Include(s => (s as SubComment).Comment).Include(s => (s as PostComment).Post).OrderByDescending(i => i.Date);
            return _mapper.Map<PagedList<Comment>, PagedList<CommentOutput>>(await PagedList<Comment>.CreatePagedListAsync(Query, input.PageNumber, input.PageSize));
        }
        public async Task<PagedList<CommentOutput>> GetOnAccountComments(Pagination input, string AccountUserName)
        {
            var Query = base.GetQuery().Include(s => s.User).Where(s => (s as AccountComment).OnAccount.NormalizedUserName.Equals(AccountUserName.ToUpper())).OrderByDescending(i => i.Date);
            return _mapper.Map<PagedList<Comment>, PagedList<CommentOutput>>(await PagedList<Comment>.CreatePagedListAsync(Query, input.PageNumber, input.PageSize));

        }
        public async Task<PagedList<CommentOutput>> GetOnPostComments(Pagination input, int Postid)
        {
            var Query = base.GetQuery().Include(s => s.User).Where(s => (s as PostComment).PostId == Postid).OrderByDescending(i => i.Date);
            return _mapper.Map<PagedList<Comment>, PagedList<CommentOutput>>(await PagedList<Comment>.CreatePagedListAsync(Query, input.PageNumber, input.PageSize));
        }
        public async Task<PagedList<CommentOutput>> GetSubComments(Pagination input, int Commentid)
        {
            var Query = base.GetQuery().Include(s => s.User).Where(s => (s as SubComment).CommentId == Commentid).OrderByDescending(i => i.Date);
            return _mapper.Map<PagedList<Comment>, PagedList<CommentOutput>>(await PagedList<Comment>.CreatePagedListAsync(Query, input.PageNumber, input.PageSize));
        }

    }
    public interface ICommentService
    {
        public Task<ResponseService<CommentOutput>> CreatePostComment(PostCommentCreateInput input, User user);
        public Task<ResponseService<CommentOutput>> CreateAccountComment(AccountCommentCreateInput input, User user);
        public Task<ResponseService<CommentOutput>> CreateSubComment(SubCommentCreateInput input, User user);
        public Task<ResponseService<CommentOutput>> GetComment(int id);
        public Task<ResponseService<CommentOutput>> Update(CommentUpdateInput Input, User user);
        public Task<PagedList<CommentOutput>> GetMyComments(Pagination input, User User);
        public Task<PagedList<CommentOutput>> GetOnAccountComments(Pagination input, string AccountUserName);
        public Task<PagedList<CommentOutput>> GetOnPostComments(Pagination input, int Postid);
        public  Task<PagedList<CommentOutput>> GetSubComments(Pagination input, int Commentid);
        public Task<ResponseService<bool>> delete(int id, User user);
    }
}
