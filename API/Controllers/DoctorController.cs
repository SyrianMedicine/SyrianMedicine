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


        [Authorize]
        [HttpGet(nameof(Doctors))]
        public async Task<IReadOnlyList<DoctorsOutput>> Doctors()
            => await _unitOfWork.DoctorServices.GetAllDoctors();
            
        [Authorize]
        [HttpGet(nameof(Doctor))]
        public async Task<DoctorsOutput> Doctor(int id)
            => await _unitOfWork.DoctorServices.GetDoctor(id);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterDoctor))]
        public async Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor([FromForm] RegisterDoctor input)
            => await _unitOfWork.DoctorServices.RegisterDoctor(input);

        [AllowAnonymous]
        [HttpPost(nameof(LoginDoctor))]
        public async Task<ResponseService<LoginOutput>> LoginDoctor(LoginInput input)
            => await _unitOfWork.DoctorServices.LoginDoctor(input);
    }
}