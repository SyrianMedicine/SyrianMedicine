using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Common;
using Services;
using Microsoft.AspNetCore.Mvc;
using DAL.Entities;
using Services.Common;
using Models.Tag.Output;
using Models.Tag.Input;
using Microsoft.AspNetCore.Authorization;
using Models.Helper;
using Models.Post.Output;

namespace API.Controllers
{
    /// <summary>
    /// used as Keyword in post 
    /// </summary>
    public class TagController : BaseController
    {
        public TagController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// get all the tag in DataBase
        /// </summary>
        /// <returns>List of tag object </returns>
        [HttpGet(nameof(GetAllTags))]
        public async Task<ActionResult<ResponseService<List<TagOutput>>>> GetAllTags() =>
           Result(await _unitOfWork.TagService.GetAllTags());

        /// <summary>
        /// get tag that have the passed id
        /// </summary>
        /// <param name="Id">id of the tag</param>
        /// <returns>(TagOutput) tag object </returns>
        [HttpGet(nameof(GetTag))]
        public async Task<ActionResult<ResponseService<TagOutput>>> GetTag(int Id) =>
            Result(await _unitOfWork.TagService.GetTag(Id));

        /// <summary>
        /// search for tags that Contains the Query string
        /// </summary>
        /// <param name="Query">is string that ,we need all  tags  Contained it</param>
        /// <returns>list of tags that Contains Query string</returns>
        [HttpGet(nameof(Search))]
        public async Task<ActionResult<ResponseService<List<TagOutput>>>> Search(string Query) =>
           Result(await _unitOfWork.TagService.SearchTag(Query));

        /// <summary>
        /// Create new tag to use
        /// </summary>
        /// <param name="Input">
        /// name of tag as JSON
        /// {name:'namevalue'}
        /// </param>
        /// <returns><h1>same name value and the tag id</h1></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(Create))]
        public async Task<ActionResult<ResponseService<TagOutput>>> Create(TagCreateInput Input) =>
            Result(await _unitOfWork.TagService.CreateTag(Input), nameof(Create));

        /// <summary>
        /// update old Tag name
        /// </summary>
        /// <param name="Input">old Id and new tag name as json  </param>
        /// <returns>the same object passed to it ^-^</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost(nameof(Update))]
        public async Task<ActionResult<ResponseService<TagOutput>>> Update(TagUpdateInput Input) =>
           Result(await _unitOfWork.TagService.UpdateTag(Input));

        /// <summary>
        /// delete tag from database
        /// </summary>
        /// <param name="Id">id number </param>
        /// <returns>true for success or false for fail</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete(nameof(Delete))]
        public async Task<ActionResult<ResponseService<bool>>> Delete(int Id) =>
            Result(await _unitOfWork.TagService.DeleteTag(Id));
        /// <summary>
        /// get posts related to this tag
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/posts")]
        public async Task<PagedList<PostOutput>> GetTagPosts(DynamicPagination input, int id) =>
           await _unitOfWork.PostService.GetTagPosts(input, id);
    }
}