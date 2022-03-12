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
using Models.Rating.Input;
using Models.Rating.Output;
using Services.Common;

namespace Services.Services
{
    public class RatingService : GenericRepository<Rating>, IRatingService
    {
        private IIdentityRepository _IdentityRepository { get; }
        public RatingService(IIdentityRepository _IdentityRepository, StoreContext dbContext, IMapper _mapper) : base(dbContext, _mapper)
        {
            this._IdentityRepository = _IdentityRepository;
        }

        public async Task<ResponseService<RatingOutput>> GetUserRating(string Username)
        {
            try
            {

                var result = new ResponseService<RatingOutput>();
                var rateduser = await _IdentityRepository.GetUserByNameAsync(Username);
                if (rateduser == null)
                    return result.SetMessage("user Not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (!IsUserRatable(rateduser))
                    return result.SetMessage("You cannot Rate this user").SetStatus(StatusCodes.Forbidden.ToString());
                result.Data = new();
                result.Data.RatingData = await base.GetQuery().Where(i => i.RatedUserid.Equals(rateduser.Id)).GroupBy(i => i.RateValue).Select(i => new RatingOutput.RatingOutputRaw { StarNumber = i.Key, Count = i.Count() }).ToListAsync();
                return result.SetMessage("OK ").SetStatus(StatusCodes.Ok.ToString());
            }
            catch
            {
                return ResponseService<RatingOutput>.GetExeptionResponse();
            }

        }

        private bool IsUserRatable(User user) => user != null &&
        (
            user.UserType == DAL.Entities.Identity.Enums.UserType.Doctor ||
            user.UserType == DAL.Entities.Identity.Enums.UserType.Nurse ||
            user.UserType == DAL.Entities.Identity.Enums.UserType.Hospital
        );

        public async Task<ResponseService<bool>> Rate(RatingInput input, User user)
        {
            try
            {

                var result = new ResponseService<bool>() { Data = false };
                var rateduser = await _IdentityRepository.GetUserByNameAsync(input.Username);
                if (rateduser == null)
                    return result.SetMessage("user Not Found").SetStatus(StatusCodes.NotFound.ToString());
                if (!IsUserRatable(rateduser))
                    return result.SetMessage("You cannot Rate this user").SetStatus(StatusCodes.Forbidden.ToString());
                DAL.Entities.Rating rate = await base.GetQuery().Where(i => i.RatedUserid.Equals(rateduser.Id) && i.userid.Equals(user.Id)).FirstOrDefaultAsync();
                if (rate == default)
                {
                    rate = new Rating { RatedUserid = rateduser.Id, userid = user.Id, RateValue = input.StarsNumber };
                    await base.InsertAsync(rate);
                }
                else
                {
                    rate.RateValue = input.StarsNumber;
                    base.Update(rate);
                }
                return await base.CompleteAsync() ?
                result.SetData(true).SetMessage("Rated").SetStatus(StatusCodes.Ok.ToString())
                :

                result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }

        }
    }
    public interface IRatingService
    {
        public Task<ResponseService<bool>> Rate(RatingInput input, User user);
        public Task<ResponseService<RatingOutput>> GetUserRating(string Username);
    }
}