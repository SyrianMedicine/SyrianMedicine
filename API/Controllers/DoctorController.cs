using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Doctor.Inputs;
using Models.Doctor.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    [AllowAnonymous]
    public class DoctorController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public DoctorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet(nameof(Doctors))]
        public async Task<IReadOnlyList<DoctorsOutput>> Doctors()
            => await _unitOfWork.DoctorServices.GetAllDoctors();

        [HttpPost(nameof(RegisterDoctor))]
        public async Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor([FromForm] RegisterDoctor input)
            => await _unitOfWork.DoctorServices.RegisterDoctor(input);
    }
}