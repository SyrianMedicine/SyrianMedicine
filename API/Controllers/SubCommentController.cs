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
    public class SubCommentController : BaseController
    {
        IHubContext<PublicHub, IPublicHub> _hub;
        public SubCommentController(IHubContext<PublicHub, IPublicHub> _hub, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            this._hub = _hub;
        }
        /// <summary>
        /// GetSubComment(reply on AccountComment or PostComment)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseService<CommentOutput>>> GetSubComment(int id) =>
            Result(await _unitOfWork.SubCommentService.GetSubComment(id));

        /// <summary>
        /// reply on AccountComment or PostComment
        ///  Note: this action notfy Comment Owner on NotfiyReplyOnComment function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(Create))]
        public async Task<ActionResult<ResponseService<SubCommentOutput>>> Create(SubCommentCreateInput input)
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
        [HttpPost(nameof(Update))]
        public async Task<ActionResult<ResponseService<SubCommentOutput>>> Update(CommentUpdateInput input) =>
        Result(await _unitOfWork.SubCommentService.Update(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        ///  delete subComment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete(nameof(Delete))]
        public async Task<ActionResult<ResponseService<bool>>> Delete(int id) =>
        Result(await _unitOfWork.SubCommentService.Delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
    }
}