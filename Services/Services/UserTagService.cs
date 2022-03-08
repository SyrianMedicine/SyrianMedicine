using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.Entities;
using DAL.Repositories;
using AutoMapper;
using Services.Common;
using DAL.Entities.Identity;
using Models.Tag.Output;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class UserTagService : GenericRepository<UserTag>, IUserTagService
    { 
        private readonly  IGenericRepository<Tag> _iGenericRepositoryTag;

        public UserTagService(IGenericRepository<Tag> _iGenericRepositoryTag, IMapper mapper, StoreContext dbContext) : base(dbContext,mapper)
        { 
            this._iGenericRepositoryTag = _iGenericRepositoryTag;
        }

        public async Task<ResponseService<List<TagOutput>>> GetMylist(User user)
        {
            var result = new ResponseService<List<TagOutput>>()
            {
                Data = _mapper.Map<List<Tag>, List<TagOutput>>(await base.GetQuery().Where(i => i.UserId.Equals(user.Id)).Include(i => i.Tag).Select(j => j.Tag).ToListAsync())
            };
            return result.Data != null && result.Data.Any() ?
            result.SetMessage("ok").SetStatus(StatusCodes.Ok.ToString())
            :
            result.SetMessage("you have no Interested Tags").SetStatus(StatusCodes.NotFound.ToString());
        }

        public async Task<ResponseService<bool>> AddtoTagList(int id, User user)
        {
            var result = new ResponseService<bool> { Data = false };
            try
            {
                if (!await _iGenericRepositoryTag.GetQuery().Where(i => i.Id == id).AnyAsync())
                    return result.SetMessage("Tag Not Found").SetStatus(StatusCodes.NotFound.ToString());

                if (await base.GetQuery().Where(i => i.TagId == id && i.UserId.Equals(user.Id)).AnyAsync())
                    return result.SetMessage("this Tag already in your list").SetStatus(StatusCodes.BadRequest.ToString());

                await base.InsertAsync(new UserTag { TagId = id, UserId = user.Id });
                return await base.CompleteAsync() ?
                    result.SetData(true).SetMessage("tag added in your intrested tag").SetStatus(StatusCodes.Created.ToString())
                    :
                    result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> RemovefromTagList(int id, User user)
        {
            var result = new ResponseService<bool> { Data = false };
            try
            {
                var usrtag = await base.GetQuery().Where(i => i.TagId == id && i.UserId.Equals(user.Id)).FirstOrDefaultAsync();
                if (usrtag == null)
                    return result.SetMessage("Not Found in your list").SetStatus(StatusCodes.NotFound.ToString());
                await base.DeleteAsync(usrtag.Id);
                return await base.CompleteAsync() ?
                    result.SetData(true).SetMessage("tag deleted from your intrested tag").SetStatus(StatusCodes.Ok.ToString())
                    :
                    result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }
    }
    public interface IUserTagService
    {
        public Task<ResponseService<bool>> AddtoTagList(int id, User user);
        public Task<ResponseService<bool>> RemovefromTagList(int id, User user);
        public Task<ResponseService<List<TagOutput>>> GetMylist(User user);
         
    }
}