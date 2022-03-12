using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Rating.Input;
using Models.Rating.Output;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class RatingController : BaseController
    {
        public RatingController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        [Authorize]
        [HttpPost(nameof(Rate))]
        public async Task<ActionResult<ResponseService<bool>>> Rate(RatingInput input) =>
         Result(await _unitOfWork.RatingService.Rate(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// get yor rating info for some user  
        /// </summary>
        /// <param name="Username">user that you need to know what you are rated before</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{Username}/MyRating")]
        public async Task<ActionResult<ResponseService<RatingOutput>>> GetMyRatingForUser(string Username) =>
             Result(await _unitOfWork.RatingService.GetMyRatingForUser(Username, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// get the user rating 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        [HttpGet("{Username}")]
        public async Task<ActionResult<ResponseService<RatingOutput>>> GetUserRating(string Username) =>
            Result(await _unitOfWork.RatingService.GetUserRating(Username));
    }
}