using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Admin.Inputs;
using Models.Admin.Outputs;
using Models.Common;
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

        [HttpGet(nameof(GetCities))]
        public async Task<IReadOnlyList<OptionDto>> GetCities()
            => await _unitOfWork.AccountService.GetCities();

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
    }
}