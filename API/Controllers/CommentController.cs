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
        private readonly IUnitOfWork _unitOfWork;
        IHubContext<PublicHub, IPublicHub> _hub;
        public CommentController(IHubContext<PublicHub, IPublicHub> _hub, IUnitOfWork unitOfWork)
        {
            this._hub = _hub;
            this._unitOfWork = unitOfWork;
        }
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
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseService<CommentOutput>>> GetComment(int id) =>
         Result(await _unitOfWork.CommentService.GetComment(id));

        [Authorize]
        [HttpPost(nameof(Update))]
        public async Task<ActionResult<ResponseService<CommentOutput>>> Update(CommentUpdateInput input) =>
            Result(await _unitOfWork.CommentService.Update(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        [Authorize]
        [HttpDelete(nameof(Delete))]
        public async Task<ActionResult<ResponseService<bool>>> Delete(int id) =>
        Result(await _unitOfWork.CommentService.delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

    }
}