using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Doctor.Inputs;
using Models.Doctor.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class DoctorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public DoctorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet(nameof(Doctors))]
        public async Task<IReadOnlyList<DoctorOutput>> Doctors()
            => await _unitOfWork.DoctorServices.GetAllDoctors();

        [HttpGet("{username}")]
        public async Task<DoctorOutput> Doctor(string username)
            => await _unitOfWork.DoctorServices.GetDoctor(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterDoctor))]
        public async Task<ActionResult<ResponseService<RegisterDoctorOutput>>> RegisterDoctor([FromForm] RegisterDoctor input)
            => Result(await _unitOfWork.DoctorServices.RegisterDoctor(input), nameof(RegisterDoctor));

        [AllowAnonymous]
        [HttpPost(nameof(LoginDoctor))]
        public async Task<ActionResult<ResponseService<LoginOutput>>> LoginDoctor(LoginDoctorInput input)
            => Result(await _unitOfWork.DoctorServices.LoginDoctor(input), nameof(LoginDoctor));
    }
}