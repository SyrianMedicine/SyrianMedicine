using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Hospital.Inputs;
using Models.Hospital.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class HospitalController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public HospitalController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet(nameof(Hospitals))]
        public async Task<IReadOnlyList<HospitalOutput>> Hospitals()
            => await _unitOfWork.HospitalServices.GetAllHospitals();

        [HttpGet("{username}")]
        public async Task<HospitalOutput> Hospital(string username)
            => await _unitOfWork.HospitalServices.GetHospital(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterHospital))]
        public async Task<ActionResult<ResponseService<RegisterHospitalOutput>>> RegisterHospital([FromForm] RegisterHospital input)
            => Result(await _unitOfWork.HospitalServices.RegisterHospital(input), nameof(RegisterHospital));

        [AllowAnonymous]
        [HttpPost(nameof(LoginHospital))]
        public async Task<ActionResult<ResponseService<RegisterHospitalOutput>>> LoginHospital(LoginHospital input)
            => Result(await _unitOfWork.HospitalServices.LoginHospital(input), nameof(LoginHospital));

    }
}