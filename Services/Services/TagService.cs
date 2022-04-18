using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Tag.Output;
using Models.Tag.Input;
using Services.Common;
using DAL.Entities;
using DAL.Repositories;
using DAL.DataContext;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class TagService : GenericRepository<Tag>, ITagService
    {
        public TagService(IMapper mapper, StoreContext dbContext) : base(dbContext, mapper)
        {

        }

        public async Task<ResponseService<List<TagOutput>>> GetAllTags()
        {
            var result = new ResponseService<List<TagOutput>>()
            {
                Data = _mapper.Map<List<Tag>, List<TagOutput>>(await base.GetQuery().ToListAsync())
            };
            return result.Data != null && result.Data.Any() ?
             result.SetMessage("Ok").SetStatus(StatusCodes.Ok.ToString()) :
             result.SetMessage("Ooops, no yag added now").SetStatus(StatusCodes.NotFound.ToString());
        }

        public async Task<ResponseService<TagOutput>> GetTag(int id)
        {
            var result = new ResponseService<TagOutput>()
            {
                Data = _mapper.Map<Tag, TagOutput>(await base.GetByIdAsync(id))
            };
            return result.Data != null ?
             result.SetMessage("Ok").SetStatus(StatusCodes.Ok.ToString())
             :
             result.SetMessage("Tag not found").SetStatus(StatusCodes.NotFound.ToString());
        }

        public async Task<ResponseService<List<TagOutput>>> SearchTag(string Query)
        {
            var result = new ResponseService<List<TagOutput>>()
            {
                Data = _mapper.Map<List<Tag>, List<TagOutput>>(await base.GetQuery().Where(i => i.Tagname.Contains(Query)).Take(6).ToListAsync())
            };
            return result.Data.Any() ?
             result.SetMessage("Ok").SetStatus(StatusCodes.Ok.ToString())
             :
             result.SetMessage("No content").SetStatus(StatusCodes.NotFound.ToString());
        }

        public async Task<ResponseService<TagOutput>> CreateTag(TagCreateInput Input)
        {
            var tag = _mapper.Map<TagCreateInput, Tag>(Input);
            var result = new ResponseService<TagOutput>();
            try
            {
                if (await base.GetQuery().Where(s => s.Tagname.Equals(Input.Name)).AnyAsync())
                {
                    result.SetMessage("Tag already exists").SetStatus(StatusCodes.BadRequest.ToString());
                }
                await base.InsertAsync(tag);
                return await base.CompleteAsync() ?
                    result.SetMessage("Tag created").SetData(_mapper.Map<Tag, TagOutput>(tag)).SetStatus(StatusCodes.Created.ToString())
                    :
                    result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<TagOutput>.GetExeptionResponse();
            }
        }
        public async Task<ResponseService<TagOutput>> UpdateTag(TagUpdateInput Input)
        {
            var tag = _mapper.Map<TagUpdateInput, Tag>(Input);
            var result = new ResponseService<TagOutput>();
            if (!await base.GetQuery().Where(i => i.Id == Input.Id).AnyAsync())
                return result.SetMessage("not found").SetStatus(StatusCodes.NotFound.ToString());
            try
            {

                base.Update(tag);
                return await base.CompleteAsync() ?
                    result.SetMessage("Tag updated").SetData(_mapper.Map<Tag, TagOutput>(tag)).SetStatus(StatusCodes.Accepted.ToString())
                    :
                    result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<TagOutput>.GetExeptionResponse();
            }

        }

        public async Task<ResponseService<bool>> DeleteTag(int Id)
        {

            var result = new ResponseService<bool>() { Data = false };
            try
            {
                if (!await base.GetQuery().Where(i => i.Id == Id).AnyAsync())
                    return result.SetMessage("NotFound").SetStatus(StatusCodes.NotFound.ToString());
                if (await base.GetQuery().Select(i => i.Users.Any() || i.Posts.Any()).FirstOrDefaultAsync())
                {
                    return result.SetMessage("this tag is used").SetStatus(StatusCodes.BadRequest.ToString());
                }

                await base.DeleteAsync(Id);
                return await base.CompleteAsync() ?
                    result.SetData(true).SetMessage("Deleted").SetStatus(StatusCodes.Ok.ToString())
                    :
                    result.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }
    }
    public interface ITagService
    {
        public Task<ResponseService<TagOutput>> GetTag(int id);
        public Task<ResponseService<List<TagOutput>>> GetAllTags();
        public Task<ResponseService<List<TagOutput>>> SearchTag(String Query);
        public Task<ResponseService<TagOutput>> CreateTag(TagCreateInput Input);
        public Task<ResponseService<TagOutput>> UpdateTag(TagUpdateInput Input);
        public Task<ResponseService<bool>> DeleteTag(int Id);
    }
}