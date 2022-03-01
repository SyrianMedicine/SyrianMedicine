using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Follow.Output;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class FollowController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public FollowController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// follow user <br/> 
        /// </summary>
        /// <param name="username">user name for user to unfollow it</param>
        /// <returns></returns>
        [HttpPost("{username}/" + nameof(Follow))]
        public async Task<ActionResult<ResponseService<bool>>> Follow(string username) =>
            Result(await _unitOfWork.FollowService.FollowUser(username, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));


        /// <summary>
        /// unfollow user 
        /// </summary>
        /// <param name="username">user name for user to follow it</param>
        /// <returns></returns>
        [HttpPost("{username}/" + nameof(UnFollow))]
        public async Task<ActionResult<ResponseService<bool>>> UnFollow(string username) =>
            Result(await _unitOfWork.FollowService.UnFollowUser(username, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));


        [HttpGet("{username}/" + nameof(FollowingList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowingList(string username) =>
            Result(await _unitOfWork.FollowService.FollowingList(username));

        [HttpGet("{username}/" + nameof(FollowersList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowersList(string username) =>
            Result(await _unitOfWork.FollowService.FollowersList(username));

        [Authorize]
        [HttpGet(nameof(FollowingList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowingList() =>
         Result(await _unitOfWork.FollowService.FollowingList((await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).UserName));

        [Authorize]
        [HttpGet(nameof(FollowersList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowersList() =>
            Result(await _unitOfWork.FollowService.FollowersList((await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).UserName));


    }
}