using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Common;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : BaseController
    { 
        public DashboardController(IUnitOfWork unitOfWork):base(unitOfWork)
        {
           
        }

        [HttpPut(nameof(ValidateDoctor) + "/{id}")]
        public async Task<ActionResult<ResponseService<bool>>> ValidateDoctor(int id)
            => Result(await _unitOfWork.DashboardService.ValidateDoctor(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ValidateDoctor));

        [HttpPut(nameof(ValidateHospital) + "/{id}")]
        public async Task<ActionResult<ResponseService<bool>>> ValidateHospital(int id)
            => Result(await _unitOfWork.DashboardService.ValidateHospital(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ValidateHospital));

        [HttpPut(nameof(ValidateNurse) + "/{id}")]
        public async Task<ActionResult<ResponseService<bool>>> ValidateNurse(int id)
            => Result(await _unitOfWork.DashboardService.ValidateNurse(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(ValidateNurse));

    }
}