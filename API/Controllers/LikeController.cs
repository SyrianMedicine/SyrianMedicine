using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Like.Output;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class LikeController : BaseController
    {
        public LikeController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        [Authorize]
        [HttpPost(nameof(LikeComment))]
        public async Task<ActionResult<ResponseService<LikeOutput>>> LikeComment(int CommentId) =>

            Result(await _unitOfWork.LikeService.LikeComment(CommentId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        [Authorize]
        [HttpPost(nameof(LikePost))]
        public async Task<ActionResult<ResponseService<LikeOutput>>> LikePost(int PostId) =>
            Result(await _unitOfWork.LikeService.LikePost(PostId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        [Authorize]
        [HttpPost(nameof(UnLikeComment))]
        public async Task<ActionResult<ResponseService<bool>>> UnLikeComment(int CommentId) =>
            Result(await _unitOfWork.LikeService.UnLikeComment(CommentId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        [Authorize]
        [HttpPost(nameof(UnLikePost))]
        public async Task<ActionResult<ResponseService<bool>>> UnLikePost(int PostId) =>
            Result(await _unitOfWork.LikeService.UnLikePost(PostId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        /// this use to delete like by like id 
        /// you can use it for any like type
        /// </summary>
        /// <param name="LikIid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(Unlike))]
        public async Task<ActionResult<ResponseService<bool>>> Unlike(int LikIid) =>
            Result(await _unitOfWork.LikeService.Unlike(LikIid, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        [Authorize]
        [HttpPost(nameof(IsCommentliked))]
        public async Task<ActionResult<ResponseService<bool>>> IsCommentliked(int CommentId) =>

            Result(await _unitOfWork.LikeService.IsCommentliked(CommentId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        [Authorize]
        [HttpPost(nameof(IsPostliked))]
        public async Task<ActionResult<ResponseService<bool>>> IsPostliked(int PostId) =>

            Result(await _unitOfWork.LikeService.IsPostliked(PostId, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
    }
}