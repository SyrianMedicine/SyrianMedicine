using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Follow.Output;
using Services.Common;

namespace Services.Services
{
    public class FollowService : GenericRepository<Follow>, IFollowService
    {

        private readonly IIdentityRepository _identityRepository;

        public FollowService(IIdentityRepository _identityRepository, IMapper _mapper, StoreContext dbContext) : base(dbContext, _mapper)
        {
            this._identityRepository = _identityRepository;

        }

        public async Task<ResponseService<bool>> FollowUser(string username, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {
                if (user.UserName.ToUpper().Equals(username.ToUpper()))
                    return result.SetMessage("you cannot Follow yourself ^-^").SetStatus(StatusCodes.BadRequest.ToString());


                if (await base.GetQuery().Include(i => i.FollowedUser).Where(s => s.UserId.Equals(user.Id) && s.FollowedUser.UserName.ToUpper().Equals(username.ToUpper())).AnyAsync())
                    return result.SetMessage($"You are already following {username}").SetStatus(StatusCodes.BadRequest.ToString());

                var followedid = await _identityRepository.GetUsersQuery().Where(i => i.UserName.ToUpper().Equals(username.ToUpper())).Select(i => i.Id).FirstOrDefaultAsync();
                if (followedid == default)
                    return result.SetMessage($"This User: {username} is not found").SetStatus(StatusCodes.NotFound.ToString());
                await base.InsertAsync(new Follow() { FollowDate = DateTime.UtcNow, FollowedUserId = followedid, UserId = user.Id });


                return await base.CompleteAsync() ?
                result.SetData(true).SetMessage($"{username} followed").SetStatus(StatusCodes.Ok.ToString())
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }

        }

        public async Task<ResponseService<bool>> UnFollowUser(string username, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {

                if (user.UserName.ToUpper().Equals(username.ToUpper()))
                    return result.SetMessage("you cannot Follow yourself ^-^").SetStatus(StatusCodes.BadRequest.ToString());
                var followeduser = await _identityRepository.GetUserByNameAsync(username);
                if (followeduser == default)
                    return result.SetMessage($"{username} is not found").SetStatus(StatusCodes.NotFound.ToString());
                var obgtodelete = await base.GetQuery().Include(i => i.FollowedUser).Where(s => s.UserId.Equals(user.Id) && s.FollowedUser.NormalizedUserName.Equals(followeduser.NormalizedUserName)).FirstOrDefaultAsync();
                if (obgtodelete == default)
                    return result.SetMessage($"You are not Following {username}").SetStatus(StatusCodes.BadRequest.ToString());

                await base.DeleteAsync(obgtodelete.Id);
                return await base.CompleteAsync() ?
                result.SetData(true).SetMessage($"{username} unfollowed").SetStatus(StatusCodes.Ok.ToString())
                :
                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }

        }
        public async Task<ResponseService<List<FollowOutput>>> FollowersList(string username)
        {
            try
            {

                var result = new ResponseService<List<FollowOutput>>
                {
                    Data = _mapper.Map<List<Follow>, List<FollowOutput>>(await base.GetQuery().Include(s => s.User).Include(i => i.FollowedUser).Where(i => i.FollowedUser.UserName.ToUpper().Equals(username.ToUpper())).ToListAsync())
                };
                return result.Data != null && result.Data.Any() ?
                    result.SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
                    :
                    result.SetMessage("You are not followed by any one >_<").SetStatus(StatusCodes.NotFound.ToString());

            }
            catch
            {
                return ResponseService<List<FollowOutput>>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<List<FollowOutput>>> FollowingList(string username)
        {
            try
            {
                var result = new ResponseService<List<FollowOutput>>
                {
                    Data = _mapper.Map<List<Follow>, List<FollowOutput>>(await base.GetQuery().Include(i => i.User).Include(s => s.FollowedUser).Where(i => i.User.UserName.ToUpper().Equals(username.ToUpper())).ToListAsync())
                };
                return result.Data != null && result.Data.Any() ?
                result.SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
                :
                result.SetMessage("you are not following any one").SetStatus(StatusCodes.NotFound.ToString());
            }
            catch
            {
                return ResponseService<List<FollowOutput>>.GetExeptionResponse();
            }
        }
        public async Task<ResponseService<bool>> IsFollowedByMe(String username, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {

                if (user.UserName.ToUpper().Equals(username.ToUpper()))
                    return result.SetMessage("You cannot follow yourself ^-^").SetStatus(StatusCodes.BadRequest.ToString());
                var followeduser = await _identityRepository.GetUserByNameAsync(username);
                if (followeduser == default)
                    return result.SetMessage(username + " not Found").SetStatus(StatusCodes.NotFound.ToString());
                var obgtodelete = await base.GetQuery().Include(i => i.FollowedUser).Where(s => s.UserId.Equals(user.Id) && s.FollowedUser.NormalizedUserName.Equals(followeduser.NormalizedUserName)).FirstOrDefaultAsync();
                return obgtodelete != default ?
                result.SetData(true).SetMessage("You are  following " + username).SetStatus(StatusCodes.Ok.ToString())
                :
                result.SetData(false).SetMessage("You are not following " + username).SetStatus(StatusCodes.Ok.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

    }
    public interface IFollowService
    {
        public Task<ResponseService<bool>> FollowUser(String username, User user);
        public Task<ResponseService<bool>> UnFollowUser(String username, User user);
        public Task<ResponseService<bool>> IsFollowedByMe(String username, User user);
        public Task<ResponseService<List<FollowOutput>>> FollowingList(string username);
        public Task<ResponseService<List<FollowOutput>>> FollowersList(string username);
    }
}