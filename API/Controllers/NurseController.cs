using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Doctor.Outputs;
using Models.Helper;
using Models.Nurse.Inputs;
using Models.Nurse.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class NurseController : BaseController
    {
        public NurseController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }


        [HttpGet(nameof(Nurses))]
        public async Task<IReadOnlyList<NurseOutput>> Nurses()
            => await _unitOfWork.NurseServices.GetAllNurses();

        [HttpPost(nameof(PaginationNurses))]
        public async Task<PagedList<NurseOutput>> PaginationNurses(NurseQuery input)
            => await _unitOfWork.NurseServices.GetPaginationNurse(input);

        [HttpPost(nameof(MostNursesRated))]
        public async Task<PagedList<MostNursesRated>> MostNursesRated(NurseQuery input)
            => await _unitOfWork.NurseServices.GetMostNursesRated(input);

        [HttpGet("{username}")]
        public async Task<NurseOutput> Nurse(string username)
            => await _unitOfWork.NurseServices.GetNurse(username);

        [HttpPost(nameof(RegisterNurse))]
        public async Task<ActionResult<ResponseService<RegisterNurseOutput>>> RegisterNurse([FromForm] RegisterNurse input)
            => Result(await _unitOfWork.NurseServices.RegisterNurse(input), nameof(RegisterNurse));

        // [HttpPost(nameof(LoginNurse))]
        // public async Task<ActionResult<ResponseService<LoginOutput>>> LoginNurse(LoginNurseInput input)
        //     => Result(await _unitOfWork.NurseServices.LoginNurse(input), nameof(LoginNurse));

        [HttpPost(nameof(UpdateNurse)), Authorize(Roles = "Nurse , Sick")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateNurse(UpdateNurse input)
            => Result(await _unitOfWork.NurseServices.UpdateNurse(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateNurse));

        // [HttpGet(nameof(GetAllReversedForNurse) + "/{id}"), Authorize(Roles = "Nurse")]
        // public async Task<IReadOnlyList<ReserveNurseOutput>> GetAllReversedForNurse(int id)
        //     => await _unitOfWork.NurseServices.GetAllReversedForNurse(id);

        [HttpPost(nameof(CheckReserve)), Authorize(Roles = "Nurse")]
        public async Task<ActionResult<ResponseService<bool>>> CheckReserve(CheckReserve input)
            => Result(await _unitOfWork.NurseServices.CheckReserve(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(CheckReserve));
        
        [HttpPost(nameof(GetReserveNurseData)), Authorize(Roles = "Nurse")]
        public async Task<PagedList<ReserveNurseData>> GetReserveNurseData(ReserveNurseDataInput input)
            => await _unitOfWork.NurseServices.GetReserveNurseData(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User));
    
    }
}