using API.Controllers.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Bed.Inputs;
using Models.Bed.Outputs;
using Models.Department.Inputs;
using Models.Department.Outputs;
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

        [HttpPost(nameof(UpdateHospital)), Authorize(Roles = "Hospital , Sick")]
        public async Task<ActionResult<ResponseService<bool>>> UpdateHospital(UpdateHospital input)
            => Result(await _unitOfWork.HospitalServices.UpdateHospital(input, (await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).Id), nameof(UpdateHospital));

        [AllowAnonymous]
        [HttpPost(nameof(LoginHospital))]
        public async Task<ActionResult<ResponseService<RegisterHospitalOutput>>> LoginHospital(LoginHospital input)
            => Result(await _unitOfWork.HospitalServices.LoginHospital(input), nameof(LoginHospital));

        [HttpPost(nameof(AddDebartmentsToHospital))]
        public async Task<ActionResult<ResponseService<bool>>> AddDebartmentsToHospital(List<CreateDepartment> inputs)
            => Result(await _unitOfWork.HospitalServices.AddDebartmentsToHospital(inputs), nameof(AddDebartmentsToHospital));

        [HttpGet("DepartmentsFor/{username}")]
        public async Task<IReadOnlyList<DepartmentOutput>> GetDepartmentsForHospital(string username)
            => await _unitOfWork.HospitalServices.GetDepartmentsForHospital(username);

        [HttpGet("Department/{id}")]
        public async Task<DepartmentOutput> GetDepartments(int id)
            => await _unitOfWork.HospitalServices.GetDepartment(id);

        [HttpPost(nameof(AddBedsToDepartments))]
        public async Task<ActionResult<ResponseService<bool>>> AddBedsToDepartments(List<CreateBed> inputs)
            => Result(await _unitOfWork.HospitalServices.AddBedsToDepartment(inputs), nameof(AddBedsToDepartments));

        [HttpPost(nameof(UpdateDepartment))]
        public async Task<ActionResult<ResponseService<bool>>> UpdateDepartment(UpdateDepartment input)
            => Result(await _unitOfWork.HospitalServices.UpdateDepartment(input, (await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).Id), nameof(UpdateDepartment));

        [HttpDelete(nameof(DeleteDepartment))]
        public async Task<ActionResult<ResponseService<bool>>> DeleteDepartment(int id)
            => Result(await _unitOfWork.HospitalServices.DeleteDepartment(id, (await _unitOfWork.IdentityRepository.GetUserByUserClaim(HttpContext.User)).Id), nameof(DeleteDepartment));

        [HttpGet("BedFor/{departmentId}")]
        public async Task<IReadOnlyList<BedOutput>> GetBedsForDepartment(int departmentId)
            => await _unitOfWork.HospitalServices.GetBedsForDepartment(departmentId);

        [HttpGet("Bed/{id}")]
        public async Task<BedOutput> GetBed(int id)
            => await _unitOfWork.HospitalServices.GetBed(id);
    }
}