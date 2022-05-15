using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Bed.Inputs;
using Models.Bed.Outputs;
using Models.Department.Inputs;
using Models.Department.Outputs;
using Models.Helper;
using Models.Hospital.Inputs;
using Models.Hospital.Outputs;
using Services;
using Services.Common;

namespace API.Controllers
{
    public class HospitalController : BaseController
    {
        public HospitalController(IUnitOfWork unitOfWork) : base(unitOfWork)
        { }

        [HttpGet(nameof(Hospitals))]
        public async Task<IReadOnlyList<HospitalOutput>> Hospitals()
            => await _unitOfWork.HospitalServices.GetAllHospitals();

        [HttpPost(nameof(PaginationHospitals))]
        public async Task<PagedList<HospitalOutput>> PaginationHospitals(HospitalQuery input)
            => await _unitOfWork.HospitalServices.GetPaginationHospital(input);

        [HttpPost(nameof(MostHospitalsRated))]
        public async Task<PagedList<MostHospitalsRated>> MostHospitalsRated(HospitalQuery input)
            => await _unitOfWork.HospitalServices.GetMostHospitalsRated(input);

        [HttpGet("{username}")]
        public async Task<HospitalOutput> Hospital(string username)
            => await _unitOfWork.HospitalServices.GetHospital(username);

        [AllowAnonymous]
        [HttpPost(nameof(RegisterHospital))]
        public async Task<ActionResult<ResponseService<RegisterHospitalOutput>>> RegisterHospital([FromForm] RegisterHospital input)
            => Result(await _unitOfWork.HospitalServices.RegisterHospital(input), nameof(RegisterHospital));

        [HttpPost(nameof(UpdateHospital)), Authorize(Roles = "Hospital , Sick")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateHospital(UpdateHospital input)
            => Result(await _unitOfWork.HospitalServices.UpdateHospital(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateHospital));

        // [AllowAnonymous]
        // [HttpPost(nameof(LoginHospital))]
        // public async Task<ActionResult<ResponseService<RegisterHospitalOutput>>> LoginHospital(LoginHospital input)
        //     => Result(await _unitOfWork.HospitalServices.LoginHospital(input), nameof(LoginHospital));

        [HttpPost(nameof(AddDebartmentsToHospital)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> AddDebartmentsToHospital(List<CreateDepartment> inputs)
            => Result(await _unitOfWork.HospitalServices.AddDebartmentsToHospital(inputs, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(AddDebartmentsToHospital));

        [HttpGet("DepartmentsFor/{username}")]
        public async Task<IReadOnlyList<DepartmentOutput>> GetDepartmentsForHospital(string username)
            => await _unitOfWork.HospitalServices.GetDepartmentsForHospital(username);

        [HttpGet("Department/{id}")]
        public async Task<DepartmentOutput> GetDepartments(int id)
            => await _unitOfWork.HospitalServices.GetDepartment(id);

        [HttpPost(nameof(AddBedsToDepartments)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> AddBedsToDepartments(List<CreateBed> inputs)
            => Result(await _unitOfWork.HospitalServices.AddBedsToDepartment(inputs, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(AddBedsToDepartments));

        [HttpPost(nameof(UpdateDepartment)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateDepartment(UpdateDepartment input)
            => Result(await _unitOfWork.HospitalServices.UpdateDepartment(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateDepartment));

        [HttpDelete(nameof(DeleteDepartment)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> DeleteDepartment(int id)
            => Result(await _unitOfWork.HospitalServices.DeleteDepartment(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(DeleteDepartment));

        [HttpPost(nameof(GetBedsForHospital))]
        public async Task<IReadOnlyList<BedOutput>> GetBedsForHospital(BedForHospitalInput input)
            => await _unitOfWork.HospitalServices.GetBedsForHospital(input);

        [HttpGet("BedFor/{departmentId}")]
        public async Task<IReadOnlyList<BedOutput>> GetBedsForDepartment(int departmentId)
            => await _unitOfWork.HospitalServices.GetBedsForDepartment(departmentId);

        [HttpGet("Bed/{id}")]
        public async Task<BedOutput> GetBed(int id)
            => await _unitOfWork.HospitalServices.GetBed(id);

        [HttpPost(nameof(UpdateBed)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateBed(UpdateBed input)
            => Result(await _unitOfWork.HospitalServices.UpdateBed(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(UpdateBed));

        [HttpDelete(nameof(DeleteBed)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> DeleteBed(int id)
             => Result(await _unitOfWork.HospitalServices.DeleteBed(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(DeleteBed));

        [HttpPost(nameof(CheckReserve)), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> CheckReserve(CheckReserveHospital input)
            => Result(await _unitOfWork.HospitalServices.CheckReserve(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(CheckReserve));

        [HttpPost(nameof(MoveReserveToHistory) + "/{id}"), Authorize(Roles = "Hospital")]
        public async Task<ActionResult<ResponseService<bool>>> MoveReserveToHistory(int id)
            => Result(await _unitOfWork.HospitalServices.MoveReserveToHistory(id, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)), nameof(MoveReserveToHistory));
        
        [HttpPost(nameof(GetReserveHospitalData)), Authorize(Roles = "Hospital")]
        public async Task<PagedList<ReserveHospitalData>> GetReserveHospitalData(Pagination input)
            => await _unitOfWork.HospitalServices.GetReserveHospitalData(input, await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User));
    }
}