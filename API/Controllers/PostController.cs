using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using API.Hubs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models.Post.Input;
using Models.Post.Output;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class PostController : BaseController
    {
        private readonly IHubContext<PublicHub, IPublicHub> _hub;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public PostController(IMapper _mapper, IHubContext<PublicHub, IPublicHub> _hub, IUnitOfWork unitOfWork)
        {
            this._mapper = _mapper;
            this._hub = _hub;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Create new post <br/>
        /// Note: tags id must be related to tag within our database or you will get badRequst
        /// </summary>
        /// <param name="post">PostCreateInput as JSON</param>
        /// <returns> PostOutput</returns>
        [Authorize]
        [HttpPost(nameof(Create))]
        public async Task<ActionResult<ResponseService<PostOutput>>> Create(PostCreateInput post)
        {
            var user = await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User);
            var result = await _unitOfWork.PostService.Create(post, user);
            if (result.isDone())
                try
                {
                    var ids = (await _unitOfWork.ConnectionService.GetUserFollowersConnection(user)).Select(s => s.ConnectionID).Distinct().ToList();
                    _hub.Clients.Clients(ids).NotfiyPostCreated(result.Data, result.Data.user.DisplayName + " add new Post");
                }
                catch { }

            return Result(result);
        }

        /// <summary>
        /// Update Post <br/>
        /// it also update post tag ,so please enter post tag that you need to relate post with them
        /// <br/>
        /// Note: Old post tag that not entered in update requist Will be deleted
        /// </summary>
        /// <param name="post">PostUpdateInput </param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(nameof(Update))]
        public async Task<ActionResult<ResponseService<PostOutput>>> Update(PostUpdateInput post) =>
             Result(await _unitOfWork.PostService.Update(post, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// delete Post
        /// </summary>
        /// <param name="id">Integer number that Identity Post</param>
        /// <returns>true or false</returns>
        [Authorize]
        [HttpDelete(nameof(Delete))]
        public async Task<ActionResult<ResponseService<bool>>> Delete(int id) =>
           Result(await _unitOfWork.PostService.Delete(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)));

        /// <summary>
        /// Get Post info
        /// </summary>
        /// <param name="id">Integer number that Identity Post</param>
        /// <returns>PostOutput</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseService<PostOutput>>> GetPost(int id) =>
             Result(await _unitOfWork.PostService.GetPost(id));



    }
}