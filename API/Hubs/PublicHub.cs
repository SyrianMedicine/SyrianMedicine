using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models.Comment.Output;
using Models.Post.Output;
using Models.UserCard;
using Services;

namespace API.Hubs
{

    [Authorize]
    public class PublicHub : Hub<IPublicHub>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PublicHub(StoreContext _dbContext, IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }
        public override Task OnConnectedAsync()
        {
            var user = _unitOfWork.IdentityRepository.GetUserByUserClaim(Context.User);
            user.Wait();
            _unitOfWork.ConnectionService.newConnection(user.Result, Context.ConnectionId, Context.GetHttpContext().Request.Headers["User-Agent"]);
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _unitOfWork.ConnectionService.delete(Context.ConnectionId).Wait();
            return base.OnDisconnectedAsync(exception);
        }
    }


    public interface IPublicHub
    {
        public Task NotfiyUserFollowYou(UserCardOutput user, string messege);
        public Task NotfiyPostCreated(PostOutput Post, string messege);
        public Task NotfiyCommentCreated(CommentOutput Comment, string messege);
        public Task NotfiyReplyOnComment(SubCommentOutput Comment, string messege);
    }
}