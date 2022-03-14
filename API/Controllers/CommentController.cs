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
using Models.Helper;
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
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseService<CommentOutput>>> GetComment(int id) =>
         Result(await _unitOfWork.CommentService.GetComment(id));

        /// <summary>
        ///  for update AccountComment or PostComment
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(Update))]
        public async Task<ActionResult<ResponseService<CommentOutput>>> Update(CommentUpdateInput input) =>
            Result(await _unitOfWork.CommentService.Update(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        /// for delete AccountComment or PostComment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete(nameof(Delete))]
        public async Task<ActionResult<ResponseService<bool>>> Delete(int id) =>
        Result(await _unitOfWork.CommentService.delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// get SubComment for this comment
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Commentid"></param>
        /// <returns></returns>
        [HttpPost("{Commentid}/SubComments")]
        public async Task<PagedList<SubCommentOutput>> GetSubCommentsforComment(Pagination input, int Commentid) =>
          await _unitOfWork.SubCommentService.GetSubCommentsforComment(input, Commentid);
        /// <summary>
        /// who like this Comments
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/Liks")]
        public async Task<PagedList<Models.Like.Output.LikeOutput>> GetLiks(Pagination input, int id) =>
            await _unitOfWork.LikeService.GetCommentLiks(input, id);

    }
}