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
        private readonly IMapper _mapper;

        private readonly IIdentityRepository _identityRepository;

        public FollowService(IIdentityRepository _identityRepository, IMapper _mapper, StoreContext dbContext) : base(dbContext)
        {
            this._identityRepository = _identityRepository;
            this._mapper = _mapper;
        }

        public async Task<ResponseService<bool>> FollowUser(string username, User user)
        {
            var result = new ResponseService<bool>() { Data = false };
            try
            {
                if (user.UserName.ToUpper().Equals(username.ToUpper()))
                    return result.SetMessage("you cannot Follow yourself ^-^").SetStatus(StatusCodes.BadRequest.ToString());


                if (await base.GetQuery().Include(i => i.FollowedUser).Where(s => s.UserId.Equals(user.Id) && s.FollowedUser.UserName.ToUpper().Equals(username.ToUpper())).AnyAsync())
                    return result.SetMessage("you are already Follow " + username).SetStatus(StatusCodes.BadRequest.ToString());

                var followedid = await _identityRepository.GetUsersQuery().Where(i => i.UserName.ToUpper().Equals(username.ToUpper())).Select(i => i.Id).FirstOrDefaultAsync();
                if (followedid == default)
                    return result.SetMessage("user " + username + " notFound").SetStatus(StatusCodes.NotFound.ToString());
                await base.InsertAsync(new Follow() { FollowDate = DateTime.Now, FollowedUserId = followedid, UserId = user.Id });


                return await base.CompleteAsync() ?
                result.SetData(true).SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
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

                var obgtodelete = await base.GetQuery().Include(i => i.FollowedUser).Where(s => s.UserId.Equals(user.Id) && s.FollowedUser.UserName.ToUpper().Equals(username.ToUpper())).FirstOrDefaultAsync();

                if (obgtodelete == default)
                    return result.SetMessage("you are not Follow " + username).SetStatus(StatusCodes.BadRequest.ToString());


                await base.DeleteAsync(obgtodelete.Id);
                return await base.CompleteAsync() ?
                result.SetData(true).SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
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
                    result.SetMessage("you are not followed by any one >_<").SetStatus(StatusCodes.NotFound.ToString());

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


    }
    public interface IFollowService
    {
        public Task<ResponseService<bool>> FollowUser(String username, User user);
        public Task<ResponseService<bool>> UnFollowUser(String username, User user);
        public Task<ResponseService<List<FollowOutput>>> FollowingList(string username);
        public Task<ResponseService<List<FollowOutput>>> FollowersList(string username);
    }
}