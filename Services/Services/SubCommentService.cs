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
using Services.Common;

namespace Services.Services
{
    public class SubCommentService : GenericRepository<SubComment>, ISubCommentService
    {
        IGenericRepository<Comment> CommentRepository;
        public SubCommentService(IGenericRepository<Comment> CommentRepository, StoreContext dbContext, IMapper _mapper) : base(dbContext, _mapper)
        {
            this.CommentRepository = CommentRepository;
        }

        public async Task<ResponseService<SubCommentOutput>> Create(SubCommentCreateInput input, User user)
        {
            try
            {
                var result = new ResponseService<SubCommentOutput>();
                var Comment = _mapper.Map<SubCommentCreateInput, SubComment>(input);
                var baseComment = await CommentRepository.GetQuery().Where(i=>i.Id==input.CommentId).Include(s=>s.User).FirstOrDefaultAsync();
                if (baseComment == default) return result.SetMessage("Comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                // todo: this user maybe blocked by post or Comment Owner >_<
                Comment.UserId = user.Id;
                Comment.Comment = baseComment;
                await base.InsertAsync(Comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<SubComment, SubCommentOutput>(Comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<SubCommentOutput>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> Delete(int SubCommentid, User user)
        {
            try
            {
                var result = new ResponseService<bool>();
                var comment = await base.GetByIdAsync(SubCommentid);
                if (comment == default) return result.SetMessage("comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (comment.UserId.Equals(user.Id)) return result.SetMessage("You are not the comment Owner").SetStatus(StatusCodes.Unauthorized.ToString());
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

        public async Task<ResponseService<SubCommentOutput>> GetSubComment(int id)
        {
            var result = new ResponseService<SubCommentOutput>();
            var comment = await base.GetQuery().Include(s => s.User).Where(s => s.Id == id).Include(s => s.Comment).ThenInclude(s => s.User).FirstOrDefaultAsync();
            return comment == default ? result.SetMessage("comment not Found").SetStatus(StatusCodes.NotFound.ToString())
            :
            result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<SubComment, SubCommentOutput>(comment)).SetMessage("Done");
        }

        public async Task<ResponseService<List<SubCommentOutput>>> GetSubCommentsforComment(int Commentid)
        {
            //todo: pagination not implemented yet
            throw new NotImplementedException();
        }

        public async Task<ResponseService<SubCommentOutput>> Update(CommentUpdateInput input, User user)
        {
            try
            {
                var result = new ResponseService<SubCommentOutput>();
                var comment = await base.GetQuery().Include(s => s.User).Where(s => s.Id == input.Id).Include(s => s.Comment).ThenInclude(s => s.User).FirstOrDefaultAsync();
                if (comment == default) return result.SetMessage("comment not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (!comment.UserId.Equals(user.Id)) return result.SetMessage("You are not the comment Owner").SetStatus(StatusCodes.Unauthorized.ToString());
                comment.IsEdited = true;
                comment.CommentText = input.CommentText;
                base.Update(comment);
                return await base.CompleteAsync() ? result.SetStatus(StatusCodes.Ok.ToString()).SetData(_mapper.Map<SubComment, SubCommentOutput>(comment)).SetMessage("Done")
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<SubCommentOutput>.GetExeptionResponse();
            }
        }
    }
    public interface ISubCommentService
    {
        public Task<ResponseService<SubCommentOutput>> Create(SubCommentCreateInput input, User user);
        public Task<ResponseService<SubCommentOutput>> Update(CommentUpdateInput input, User user);
        public Task<ResponseService<bool>> Delete(int SubCommentid, User user);
        public Task<ResponseService<SubCommentOutput>> GetSubComment(int id);
        public Task<ResponseService<List<SubCommentOutput>>> GetSubCommentsforComment(int Commentid);

    }
}