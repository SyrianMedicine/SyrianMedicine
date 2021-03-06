using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.Sick.Inputs;
using Models.Sick.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class SickController : BaseController
    {
        public SickController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        [HttpGet(nameof(Sicks))]
        public async Task<IReadOnlyList<SickOutput>> Sicks()
            => await _unitOfWork.SickServices.GetAllSicks();

        [HttpGet("{username}")]
        public async Task<SickOutput> Sick(string username)
            => await _unitOfWork.SickServices.GetSick(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterSick))]
        public async Task<ActionResult<ResponseService<RegisterSickOutput>>> RegisterSick(RegisterSick input)
            => Result(await _unitOfWork.SickServices.RegisterSick(input), nameof(RegisterSick));

        // [AllowAnonymous]
        // [HttpPost(nameof(LoginSick))]
        // public async Task<ActionResult<ResponseService<LoginSickOutput>>> LoginSick(LoginSick input)
        //     => Result(await _unitOfWork.SickServices.LoginSick(input), nameof(LoginSick));

        [HttpPost(nameof(UpdateSick)), Authorize(Roles = "Sick")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateSick(UpdateSick input)
            => Result(await _unitOfWork.SickServices.UpdateSick(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateSick));

        [HttpPost(nameof(ReserveDateWithDoctor))]
        public async Task<ActionResult<ResponseService<bool>>> ReserveDateWithDoctor(ReserveDateWithDoctor input)
            => Result(await _unitOfWork.SickServices.ReserveDateWithDoctor(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ReserveDateWithDoctor));

        [HttpPost(nameof(UpdateReserveDateWithDoctor))]
        public async Task<ActionResult<ResponseService<bool>>> UpdateReserveDateWithDoctor(UpdateReserveDateWithDoctor input)
            => Result(await _unitOfWork.SickServices.UpdateReserveDateWithDoctor(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateReserveDateWithDoctor));

        [HttpDelete(nameof(DeleteReserveDateWithDoctor))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteReserveDateWithDoctor(int id)
            => Result(await _unitOfWork.SickServices.DeleteReserveDateWithDoctor(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(DeleteReserveDateWithDoctor));

        [HttpPost(nameof(ReserveDateWithNurse))]
        public async Task<ActionResult<ResponseService<bool>>> ReserveDateWithNurse(ReserveDateWithNurse input)
            => Result(await _unitOfWork.SickServices.ReserveDateWithNurse(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ReserveDateWithNurse));

        [HttpPost(nameof(UpdateReserveDateWithNurse))]
        public async Task<ActionResult<ResponseService<bool>>> UpdateReserveDateWithNurse(UpdateReserveDateWithNurse input)
            => Result(await _unitOfWork.SickServices.UpdateReserveDateWithNurse(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateReserveDateWithNurse));

        [HttpDelete(nameof(DeleteReserveDateWithNurse))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteReserveDateWithNurse(int id)
            => Result(await _unitOfWork.SickServices.DeleteReserveDateWithNurse(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(DeleteReserveDateWithNurse));

        [HttpPost(nameof(ReserveBedInHospital))]
        public async Task<ActionResult<ResponseService<bool>>> ReserveBedInHospital(ReserveBedInHospital input)
            => Result(await _unitOfWork.SickServices.ReserveBedInHospital(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ReserveBedInHospital));

        [HttpPost(nameof(UpdateReserveBedInHospital))]
        public async Task<ActionResult<ResponseService<bool>>> UpdateReserveBedInHospital(UpdateReserveBedInHospital input)
            => Result(await _unitOfWork.SickServices.UpdateReserveBedInHospital(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateReserveBedInHospital));

        [HttpDelete(nameof(DeleteReserveBedInHospital))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteReserveBedInHospital(int id)
            => Result(await _unitOfWork.SickServices.DeleteReserveBedInHospital(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(DeleteReserveBedInHospital));
        [HttpGet(nameof(GetReserveTypes))]
        public List<OptionDto> GetReserveTypes()
            => _unitOfWork.SickServices.GetReserveTypes();
    }
}
