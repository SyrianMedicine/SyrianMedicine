using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Sick.Inputs;
using Models.Sick.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class SickController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public SickController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet(nameof(Sicks))]
        public async Task<IReadOnlyList<SickOutput>> Sicks()
            => await _unitOfWork.SickServices.GetAllSicks();

        [HttpGet(nameof(Sick))]
        public async Task<SickOutput> Sick(string username)
            => await _unitOfWork.SickServices.GetSick(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterSick))]
        public async Task<ActionResult<ResponseService<RegisterSickOutput>>> RegisterSick(RegisterSick input)
            => Result(await _unitOfWork.SickServices.RegisterSick(input), nameof(RegisterSick));

        [AllowAnonymous]
        [HttpPost(nameof(LoginSick))]
        public async Task<ActionResult<ResponseService<LoginSickOutput>>> LoginSick(LoginSick input)
            => Result(await _unitOfWork.SickServices.LoginSick(input), nameof(LoginSick));
    }
}