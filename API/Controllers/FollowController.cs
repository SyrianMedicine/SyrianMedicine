using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using API.Hubs;
using AutoMapper;
using DAL.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Follow.Output;
using Models.UserCard;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class FollowController : BaseController
    {
        private readonly IHubContext<PublicHub, IPublicHub> _hub;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FollowController(IMapper _mapper, IHubContext<PublicHub, IPublicHub> _hub, IUnitOfWork unitOfWork)
        {
            this._mapper = _mapper;
            this._hub = _hub;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// follow user <br/> 
        /// Now! You will see  user  post in your Home Page and and recive Notification for user action<br/>
        /// Note: this action will send notification for user that you are  start Following it
        /// </summary>
        /// <param name="username">user name for user to unfollow it</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{username}/" + nameof(Follow))]
        public async Task<ActionResult<ResponseService<bool>>> Follow(string username)
        {
            var user = await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User);
            var result = await _unitOfWork.FollowService.FollowUser(username, user);
            if (result.isDone())
            {
                try
                {
                    var ids = (await _unitOfWork.ConnectionService.GetConnections(username)).Select(s => s.ConnectionID).Distinct().ToList();
                    var userout = _mapper.Map<DAL.Entities.Identity.User, UserCardOutput>(user);
                    _hub.Clients.Clients(ids).NotfiyUserFollowYou(userout, userout.DisplayName + " start Follow you");
                }
                catch { }
            }
            return Result(result);
        }


        /// <summary>
        /// unfollow user ,
        /// remove User from yor follow list<br/>
        /// now you cannot Know what user Do *_*
        /// </summary>
        /// <param name="username">user name for user to follow it</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{username}/" + nameof(UnFollow))]
        public async Task<ActionResult<ResponseService<bool>>> UnFollow(string username) =>
            Result(await _unitOfWork.FollowService.UnFollowUser(username, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));


        /// <summary>
        ///  users that inputuser is Following them 
        /// </summary>
        /// <returns></returns>
        [HttpGet("{username}/" + nameof(FollowingList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowingList(string username) =>
            Result(await _unitOfWork.FollowService.FollowingList(username));

        /// <summary>
        /// users that Follow inputuser 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("{username}/" + nameof(FollowersList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowersList(string username) =>
            Result(await _unitOfWork.FollowService.FollowersList(username));

        /// <summary>
        ///  users that You are Following them 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(nameof(FollowingList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowingList() =>
         Result(await _unitOfWork.FollowService.FollowingList((await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).UserName));

        /// <summary>
        /// users that Follow You 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet(nameof(FollowersList))]
        public async Task<ActionResult<ResponseService<FollowOutput>>> FollowersList() =>
            Result(await _unitOfWork.FollowService.FollowersList((await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).UserName));


    }
}