using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Comment.Input;
using Models.Comment.Output;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class CommentController : BaseController
    {
        IHubContext<PublicHub, IPublicHub> _hub;
        public CommentController(IHubContext<PublicHub, IPublicHub> _hub, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this._hub = _hub;
        }

        /// <summary>
        /// this API create Comment on post<br/>
        /// Note: this action notfy Post Owner on NotfiyCommentCreated function
        /// </summary> 
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(CreatePostComment))]
        public async Task<ActionResult<ResponseService<CommentOutput>>> CreatePostComment(PostCommentCreateInput input)
        {
            var user = await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User);
            var result = await _unitOfWork.CommentService.CreatePostComment(input, user);
            if (result.isDone())
            {
                try
                {

                    var ids = await _unitOfWork.ConnectionService.GetPostUserConnection(Convert.ToInt32(result.Data.RealtedObjectId));
                    _hub.Clients.Clients(ids.Select(i => i.ConnectionID).Distinct().ToList()).NotfiyCommentCreated(result.Data, user.FirstName + " " + user.LastName + " Comment on your Post");
                }
                catch { }
            }
            return Result(result);
        }
        /// <summary>
        /// this API create Comment on Account
        ///  Note: this action notfy Account Owner on NotfiyCommentCreated function
        /// </summary> 
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(CreateAccountComment))]
        public async Task<ActionResult<ResponseService<CommentOutput>>> CreateAccountComment(AccountCommentCreateInput input)
        {
            var user = await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User);
            var result = await _unitOfWork.CommentService.CreateAccountComment(input, user);
            if (result.isDone())
            {
                try
                {

                    var ids = await _unitOfWork.ConnectionService.GetConnections(input.AccountUserName);
                    _hub.Clients.Clients(ids.Select(i => i.ConnectionID).Distinct().ToList()).NotfiyCommentCreated(result.Data, user.FirstName + " " + user.LastName + " Comment on your Account");
                }
                catch { }
            }
            return Result(result);
        }

        /// <summary>
        /// this API get AccountComment or PostComment
        /// </summary> 
        /// <returns></returns>
        [HttpGet("Comment/{id}")]
        public async Task<ActionResult<ResponseService<CommentOutput>>> GetComment(int id) =>
         Result(await _unitOfWork.CommentService.GetComment(id));

        /// <summary>
        ///  for update AccountComment or PostComment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(UpdateComment))]
        public async Task<ActionResult<ResponseService<CommentOutput>>> UpdateComment(CommentUpdateInput input) =>
            Result(await _unitOfWork.CommentService.Update(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        /// for delete AccountComment or PostComment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete(nameof(DeleteComment))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteComment(int id) =>
        Result(await _unitOfWork.CommentService.delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// GetSubComment(reply on AccountComment or PostComment)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("subComment/{id}")]
        public async Task<ActionResult<ResponseService<CommentOutput>>> GetSubComment(int id) =>
            Result(await _unitOfWork.SubCommentService.GetSubComment(id));

        /// <summary>
        /// reply on AccountComment or PostComment
        ///  Note: this action notfy Comment Owner on NotfiyReplyOnComment function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(CreateSubComment))]
        public async Task<ActionResult<ResponseService<SubCommentOutput>>> CreateSubComment(SubCommentCreateInput input)
        {
            var user = await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User);
            var result = await _unitOfWork.SubCommentService.Create(input, user);
            if (result.isDone())
            {
                try
                {
                    var ids = await _unitOfWork.ConnectionService.GetConnections(result.Data.OnComment.User.UserName);
                    _hub.Clients.Clients(ids.Select(i => i.ConnectionID).Distinct().ToList()).NotfiyReplyOnComment(result.Data, user.FirstName + " " + user.LastName + " Reply On your Comment");
                }
                catch { }
            }
            return Result(result);
        }
        /// <summary>
        /// Update subComment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(UpdateSubComment))]
        public async Task<ActionResult<ResponseService<SubCommentOutput>>> UpdateSubComment(CommentUpdateInput input) =>
        Result(await _unitOfWork.SubCommentService.Update(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        ///  delete subComment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete(nameof(DeleteSubComment))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteSubComment(int id) =>
        Result(await _unitOfWork.SubCommentService.Delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));


    }
}