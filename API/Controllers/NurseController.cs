using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Doctor.Outputs;
using Models.Nurse.Inputs;
using Models.Nurse.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class NurseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public NurseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet(nameof(Nurses))]
        public async Task<IReadOnlyList<NurseOutput>> Nurses()
            => await _unitOfWork.NurseServices.GetAllNurses();

        [HttpGet("{username}")]
        public async Task<NurseOutput> Nurse(string username)
            => await _unitOfWork.NurseServices.GetNurse(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterNurse))]
        public async Task<ActionResult<ResponseService<RegisterNurseOutput>>> RegisterNurse([FromForm] RegisterNurse input)
            => Result(await _unitOfWork.NurseServices.RegisterNurse(input), nameof(RegisterNurse));

        [AllowAnonymous]
        [HttpPost(nameof(LoginNurse))]
        public async Task<ActionResult<ResponseService<LoginOutput>>> LoginNurse(LoginNurseInput input)
            => Result(await _unitOfWork.NurseServices.LoginNurse(input), nameof(LoginNurse));

    }
}