using API.Controllers.Common;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Admin.Inputs;
using Models.Admin.Outputs;
using Models.Common;
using Models.Helper;
using Models.User;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }


        [HttpGet(nameof(IsUserNameExist) + "/{username}")]
        public async Task<bool> IsUserNameExist(string username)
            => await _unitOfWork.AccountService.IsUserNameExist(username);

        [HttpGet(nameof(IsEmailExist) + "/{email}")]
        public async Task<bool> IsEmailExist(string email)
            => await _unitOfWork.AccountService.IsEmailExist(email);

        [HttpGet(nameof(GetCities))]
        public async Task<IReadOnlyList<OptionDto>> GetCities()
            => await _unitOfWork.AccountService.GetCities();

        [HttpGet(nameof(GetPersonStates))]
        public List<OptionDto> GetPersonStates()
            => _unitOfWork.AccountService.GetPersonStates();



        // Example For Sarye, I will delete it afer somedays
        [HttpPost(nameof(GetPaginationCities))]
        public async Task<PagedList<OptionDto>> GetPaginationCities(Pagination input)
            => await _unitOfWork.AccountService.GetCities(input);

        [HttpGet(nameof(GetGenders))]
        public List<OptionDto> GetGenders()
            => _unitOfWork.AccountService.GetGenders();

        [HttpGet(nameof(GetAccountStates))]
        public List<OptionDto> GetAccountStates()
            => _unitOfWork.AccountService.GetAccountStates();

        [HttpGet(nameof(GetRoles)), Authorize(Roles = "Admin")]
        public List<OptionDto> GetRoles()
               => _unitOfWork.AccountService.GetRoles();

        [HttpGet(nameof(GetUserTypes))]
        public List<OptionDto> GetUserTypes()
        => _unitOfWork.AccountService.GetUserTypes();

        [HttpPut(nameof(UploadImage)), Authorize]
        public async Task<ActionResult<ResponseService<bool>>> UploadImage([FromForm] UploadImage input)
            => Result(await _unitOfWork.AccountService.UploadImage(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UploadImage));

        [HttpPost(nameof(LoginAdmin))]
        public async Task<ActionResult<ResponseService<LoginAdminOutput>>> LoginAdmin(LoginInput input)
            => Result(await _unitOfWork.AccountService.LoginAdmin(input), nameof(LoginAdmin));

        [HttpPost(nameof(UpdateAdminProfile)), Authorize(Roles = "Admin")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateAdminProfile(UpdateAdmin input)
            => Result(await _unitOfWork.AccountService.UpdateAdminProfile(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateAdminProfile));

        /// <summary>
        /// get comments on Doctor or Nurses  progile
        /// </summary>
        /// <param name="input"></param>
        /// <param name="AccountUserName"></param>
        /// <returns></returns>
        [HttpPost("{AccountUserName}/Comments")]
        public async Task<Models.Helper.PagedList<Models.Comment.Output.CommentOutput>> Comments(Models.Helper.Pagination input, string AccountUserName)
            => await _unitOfWork.CommentService.GetOnAccountComments(input, AccountUserName);

        /// <summary>
        /// get user post you can display this in user profile  
        /// </summary>
        /// <param name="input"></param>
        /// <param name="AccountUserName"></param>
        /// <returns></returns>
        [HttpPost("{AccountUserName}/Posts")]
        public async Task<Models.Helper.PagedList<Models.Post.Output.PostOutput>> GetPosts(Models.Helper.Pagination input, string AccountUserName)
            => await _unitOfWork.PostService.GetUserProfilePosts(input, AccountUserName);

        /// <summary>
        /// get user Comment History 
        /// </summary>
        [Authorize]
        [HttpPost("MyCommentsHistory")]
        public async Task<Models.Helper.PagedList<Models.Comment.Output.CommentOutput>> Comments(Models.Helper.Pagination input)
            => await _unitOfWork.CommentService.GetMyComments(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User));
        /// <summary>
        /// get user Liks History 
        /// </summary>
        [Authorize]
        [HttpPost("MyLikeHistory")]
        public async Task<Models.Helper.PagedList<Models.Like.Output.LikeOutput>> MyLikeHistory(Models.Helper.Pagination input)
            => await _unitOfWork.LikeService.GetMyLikeHistory(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User));

    }
}