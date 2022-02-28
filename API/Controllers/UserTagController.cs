using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using Services;
using Microsoft.AspNetCore.Mvc;
using Services.Common;
using Models.Tag.Output;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class UserTagController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserTagController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// add tag to InterestedList of AuthorizedUser 
        /// this help us to disply posts releted to user intrested tags
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns>true for success or false for fail</returns>
        [Authorize]
        [HttpPost("{id}/" + nameof(AddtoInterestedList))]
        public async Task<ActionResult<ResponseService<bool>>> AddtoInterestedList(int id) =>
             Result(await _unitOfWork.UserTagService.InterestInTag(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(AddtoInterestedList));
        /// <summary>
        /// Remove tag from AuthorizedUser InterestedList
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns>true for success or false for fail</returns>
        [Authorize]
        [HttpDelete("{id}/" + nameof(RemovefromInterestedList))]
        public async Task<ActionResult<ResponseService<bool>>> RemovefromInterestedList(int id) =>
               Result(await _unitOfWork.UserTagService.UnInterestInTag(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        /// get tag list for Authorized user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(nameof(GetMylist))]
        public async Task<ActionResult<ResponseService<List<TagOutput>>>> GetMylist() =>
            Result(await _unitOfWork.UserTagService.InterestedTags(await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

    }
}