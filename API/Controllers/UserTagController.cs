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
        public UserTagController(IUnitOfWork unitOfWork):base(unitOfWork)
        { 
        }
        /// <summary>
        /// add tag to AuthorizedUser TagList  <br/>
        /// this help us to disply posts related to user tagList
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns>true for success or false for fail</returns>
        [Authorize]
        [HttpPost("{id}/" + nameof(AddtoTagList))]
        public async Task<ActionResult<ResponseService<bool>>> AddtoTagList(int id) =>
             Result(await _unitOfWork.UserTagService.AddtoTagList(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(AddtoTagList));
        /// <summary>
        /// Remove tag from AuthorizedUser TagList
        /// </summary>
        /// <param name="id">tag id</param>
        /// <returns>true for success or false for fail</returns>
        [Authorize]
        [HttpDelete("{id}/" + nameof(RemovefromTagList))]
        public async Task<ActionResult<ResponseService<bool>>> RemovefromTagList(int id) =>
               Result(await _unitOfWork.UserTagService.RemovefromTagList(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));
        /// <summary>
        /// get tag list for Authorized user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(nameof(GetMylist))]
        public async Task<ActionResult<ResponseService<List<TagOutput>>>> GetMylist() =>
            Result(await _unitOfWork.UserTagService.GetMylist(await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

    }
}