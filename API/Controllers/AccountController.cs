using API.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Services;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public AccountController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

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

        [HttpGet(nameof(GetRoles))]
        public List<OptionDto> GetRoles()
               => _unitOfWork.AccountService.GetRoles();

        [HttpGet(nameof(GetUserTypes))]
        public List<OptionDto> GetUserTypes()
        => _unitOfWork.AccountService.GetUserTypes();
    }
}