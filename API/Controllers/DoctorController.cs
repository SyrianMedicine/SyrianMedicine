using API.Controllers.Common;
using DAL.Entities.Identity.Enums;
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
        public DoctorController(IUnitOfWork unitOfWork):base(unitOfWork)
        {
            
        }

        [HttpGet(nameof(Doctors))]
        public async Task<IReadOnlyList<DoctorOutput>> Doctors()
            => await _unitOfWork.DoctorServices.GetAllDoctors();

        [HttpGet("{username}")]
        public async Task<DoctorOutput> Doctor(string username)
            => await _unitOfWork.DoctorServices.GetDoctor(username);

        [HttpPost(nameof(RegisterDoctor))]
        public async Task<ActionResult<ResponseService<RegisterDoctorOutput>>> RegisterDoctor([FromForm] RegisterDoctor input)
            => Result(await _unitOfWork.DoctorServices.RegisterDoctor(input), nameof(RegisterDoctor));

        [HttpPost(nameof(UpdateDoctor)), Authorize(Roles = "Doctor,Sick")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateDoctor(UpdateDoctor input)
            => Result(await _unitOfWork.DoctorServices.UpdateDoctor(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateDoctor));

        [HttpPost(nameof(LoginDoctor))]
        public async Task<ActionResult<ResponseService<LoginOutput>>> LoginDoctor(LoginDoctorInput input)
            => Result(await _unitOfWork.DoctorServices.LoginDoctor(input), nameof(LoginDoctor));

        [HttpGet(nameof(GetAllReversedForDoctor) + "/{id}"), Authorize(Roles = "Doctor")]
        public async Task<IReadOnlyList<ReserveDoctorOutput>> GetAllReversedForDoctor(int id)
            => await _unitOfWork.DoctorServices.GetAllReversedForDoctor(id);

        [HttpPost(nameof(CheckReserve)), Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ResponseService<bool>>> CheckReserve(CheckReserve input)
            => Result(await _unitOfWork.DoctorServices.CheckReserve(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(CheckReserve));

    }
}