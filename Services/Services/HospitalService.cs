using System.Data;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Bed.Inputs;
using Models.Bed.Outputs;
using Models.Department.Inputs;
using Models.Department.Outputs;
using Models.Doctor.Outputs;
using Models.Hospital;
using Models.Hospital.Inputs;
using Models.Hospital.Outputs;
using Services.Common;

namespace Services
{
    public class HospitalService : GenericRepository<Hospital>, IHospitalService
    { 
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IGenericRepository<DocumentsHospital> _documentsHospital;
        private readonly IGenericRepository<Department> _department;
        private readonly IGenericRepository<Bed> _bed;
        private readonly IGenericRepository<ReserveHospital> _reserveHospital;

        public HospitalService(IIdentityRepository identityRepository, IGenericRepository<DocumentsHospital> documentsHospital,
        IGenericRepository<Department> department, IGenericRepository<ReserveHospital> reserveHospital, IGenericRepository<Bed> bed, ITokenService tokenService, IMapper mapper, StoreContext dbContext) : base(dbContext,mapper)
        {
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _documentsHospital = documentsHospital;
            _reserveHospital = reserveHospital;
            _department = department;
            _bed = bed; 
        }

        public async Task<IReadOnlyList<HospitalOutput>> GetAllHospitals()
            => _mapper.Map<IReadOnlyList<Hospital>, IReadOnlyList<HospitalOutput>>(await GetQuery().Include(e => e.User).ToListAsync());

        public async Task<HospitalOutput> GetHospital(string username)
            => _mapper.Map<Hospital, HospitalOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.NormalizedUserName == username.ToUpper()));

        public async Task<ResponseService<RegisterHospitalOutput>> LoginHospital(LoginHospital input)
        {
            var response = new ResponseService<RegisterHospitalOutput>();
            try
            {
                var user = await _identityRepository.GetUserByEmailAsync(input.Email);
                if (user == null)
                {
                    user = await _identityRepository.GetUserByNameAsync(input.UserName);
                    if (user == null)
                    {
                        response.Message = "UserName or Email not exist!";
                        response.Status = StatusCodes.NotFound.ToString();
                        return response;
                    }
                }

                if (!await _identityRepository.CheckPassword(user, input.Password))
                {
                    response.Message = "Password not correct!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }
                var hospital = await GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);

                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                bool found = false;
                foreach (var role in roles)
                {
                    if (role == Roles.Hospital.ToString() || (hospital.AccountState == AccountState.Pending && role == Roles.Sick.ToString()))
                        found = true;
                }
                if (!found)
                {
                    response.Message = "Oooops you are not hospital";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                if (await _identityRepository.LoginUser(user, input.Password))
                {
                    response.Message = $"Welcome {hospital.Name}";
                    response.Status = StatusCodes.Ok.ToString();
                    var mapper = _mapper.Map<RegisterHospitalOutput>(user);
                    mapper.Token = await _tokenService.CreateToken(user);
                    response.Data = mapper;
                }
                else
                {
                    response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                    response.Status = StatusCodes.InternalServerError.ToString();
                    return response;
                }
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }

        public async Task<ResponseService<RegisterHospitalOutput>> RegisterHospital(RegisterHospital input)
        {
            var response = new ResponseService<RegisterHospitalOutput>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                // this user is exist 
                if (await _identityRepository.GetUserByNameAsync(input.UserName) != null || await _identityRepository.GetUserByEmailAsync(input.Email) != null)
                {
                    response.Message = "Username or Email is Exist!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }
                var files = input.Documents;
                if (files.Length == 0)
                {
                    response.Message = "Please send your document if you want register as a hospital!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                var user = _mapper.Map<User>(input);
                user.UserType = UserType.Hospital;
                var hospital = _mapper.Map<Hospital>(input);
                hospital.AccountState = AccountState.Pending;
                hospital.UserId = user.Id;

                if (await _identityRepository.CreateUserAsync(user, input.Password))
                {
                    await InsertAsync(hospital);
                    if (!await CompleteAsync())
                    {
                        response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                        response.Status = StatusCodes.InternalServerError.ToString();
                        return response;
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine("wwwroot/Documents/hospital/", "DocumentFor" + user.UserName + "_" + file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        await stream.DisposeAsync();
                        var docModel = new DocumentsHospitalModel()
                        {
                            HospitalId = hospital.Id,
                            UrlFile = path[7..]
                        };
                        var doc = _mapper.Map<DocumentsHospital>(docModel);
                        await _documentsHospital.InsertAsync(doc);
                        if (!await _documentsHospital.CompleteAsync())
                        {
                            response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                            response.Status = StatusCodes.InternalServerError.ToString();
                            return response;
                        }
                    }

                    var dbUser = await _identityRepository.GetUserByEmailAsync(input.Email);
                    await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                    response.Message = $"Welcome {hospital.Name}";
                    response.Status = StatusCodes.Created.ToString();
                    var mapper = _mapper.Map<RegisterHospitalOutput>(user);
                    mapper.Token = await _tokenService.CreateToken(user);
                    response.Data = mapper;
                    await transaction.CommitAsync();
                }
                else
                {
                    response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                    response.Status = StatusCodes.InternalServerError.ToString();
                    return response;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }

        public async Task<ResponseService<bool>> AddDebartmentsToHospital(List<CreateDepartment> inputs, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                foreach (var input in inputs)
                {
                    var department = _mapper.Map<CreateDepartment, Department>(input);
                    var hospital = await GetByIdAsync(department.HospitalId);
                    if (hospital == null)
                    {
                        return response.SetData(false).SetMessage("This hospital not exist!")
                                       .SetStatus(StatusCodes.NotFound.ToString());
                    }
                    if (hospital.UserId != user.Id)
                    {
                        return response.SetMessage("You are not authorize").SetData(false).SetStatus(StatusCodes.Unauthorized.ToString());
                    }
                    await _department.InsertAsync(department);
                }
                await transaction.CommitAsync();
                await _department.CompleteAsync();
                response.Data = true;
                response.Message = "Debartments are added";
                response.Status = StatusCodes.Created.ToString();
            }
            catch
            {
                await transaction.RollbackAsync();
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }
        public async Task<IReadOnlyList<DepartmentOutput>> GetDepartmentsForHospital(string username)
        {
            var hospital = await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.NormalizedUserName == username.ToUpper());
            if (hospital == null)
                return null;
            return _mapper.Map<IReadOnlyList<Department>, IReadOnlyList<DepartmentOutput>>
                (await _department.GetQuery().Include(e => e.Hospital).Where(e => e.HospitalId == hospital.Id).ToListAsync());
        }
        public async Task<DepartmentOutput> GetDepartment(int id)
            => _mapper.Map<Department, DepartmentOutput>(await _department.GetQuery().Include(e => e.Hospital).FirstOrDefaultAsync(e => e.Id == id));
        public async Task<ResponseService<bool>> AddBedsToDepartment(List<CreateBed> inputs, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                foreach (var input in inputs)
                {
                    var hospital = await GetByIdAsync((await _department.GetByIdAsync(input.DepartmentId)).HospitalId);
                    if (hospital.UserId != user.Id)
                    {
                        return response.SetMessage("You are not authorize").SetData(false).SetStatus(StatusCodes.Unauthorized.ToString());
                    }
                    await _bed.InsertAsync(_mapper.Map<CreateBed, Bed>(input));
                }
                await transaction.CommitAsync();
                await _department.CompleteAsync();
                response.Data = true;
                response.Message = "Beds are added";
                response.Status = StatusCodes.Created.ToString();
            }
            catch
            {
                await transaction.RollbackAsync();
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }
        public async Task<IReadOnlyList<BedOutput>> GetBedsForDepartment(int departmentId)
            => _mapper.Map<IReadOnlyList<Bed>, IReadOnlyList<BedOutput>>(await _bed.GetQuery().Include(e => e.Department).Where(e => e.DepartmentId == departmentId).ToListAsync());
        public async Task<BedOutput> GetBed(int id)
            => _mapper.Map<Bed, BedOutput>(await _bed.GetQuery().Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id));

        public async Task<ResponseService<bool>> UpdateHospital(UpdateHospital input, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var dbHospital = await GetByIdAsync(input.HospitalId);
                if (dbHospital == null)
                {
                    response.Message = "This hospital is not exist!";
                    response.Status = StatusCodes.NotFound.ToString();
                    return response;
                }

                if (user.Id != dbHospital.UserId)
                {
                    response.Message = "You are not authorized";
                    response.Status = StatusCodes.Unauthorized.ToString();
                    return response;
                }


                var userMapper = _mapper.Map(input, user);
                var hospitalMapper = _mapper.Map(input, dbHospital);

                if (await _identityRepository.UpdateUserAsync(userMapper))
                {
                    Update(hospitalMapper);
                    await CompleteAsync();
                    response.Message = "Update successed";
                    response.Status = StatusCodes.Ok.ToString();
                    response.Data = true;
                    await transaction.CommitAsync();
                }
                else
                {
                    response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                    response.Status = StatusCodes.BadRequest.ToString();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;

        }

        public async Task<ResponseService<bool>> UpdateDepartment(UpdateDepartment input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbDepartment = await _department.GetByIdAsync(input.Id);
                if (dbDepartment == null)
                {
                    return response.SetMessage("This Department is not exist !")
                        .SetStatus(StatusCodes.NotFound.ToString());
                }

                var dbHospital = await GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);
                if (dbHospital == null)
                {
                    return response.SetMessage("You not have this Department!")
                        .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                if (dbDepartment.HospitalId != dbHospital.Id)
                {
                    return response.SetMessage(@"You can't update this department")
                        .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                var department = _mapper.Map(input, dbDepartment);
                _department.Update(department);
                response = await _department.CompleteAsync() == true ?
                response.SetData(true).SetMessage("Update successed").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }

        public async Task<ResponseService<bool>> DeleteDepartment(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbDepartment = await _department.GetByIdAsync(id);
                if (dbDepartment == null)
                {
                    return response.SetMessage("This Department is not exist !")
                        .SetStatus(StatusCodes.NotFound.ToString());
                }

                var dbHospital = await GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);
                if (dbHospital == null)
                {
                    return response.SetMessage("You not have this Department!")
                        .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                if (dbDepartment.HospitalId != dbHospital.Id)
                {
                    return response.SetMessage(@"You can't update this department")
                        .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                await _department.DeleteAsync(dbDepartment.Id);
                return await _department.CompleteAsync() == true ?
                response.SetData(true).SetMessage("delete successed").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }
        public async Task<ResponseService<bool>> UpdateBed(UpdateBed input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbBed = await _bed.GetByIdAsync(input.Id);
                if (dbBed == null)
                {
                    return response.SetData(false).SetMessage("This bed not exist!")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }

                var hospital = await GetByIdAsync((await _department.GetByIdAsync(input.DepartmentId)).HospitalId);
                if (hospital.UserId != user.Id)
                {
                    return response.SetMessage("You are not authorize").SetData(false)
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                _bed.Update(_mapper.Map(input, dbBed));
                return await _bed.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Update successed").SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }

        }
        public async Task<ResponseService<bool>> DeleteBed(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var bed = await _bed.GetByIdAsync(id);
                if (bed == null)
                {
                    return response.SetMessage("This bed is not exist").SetData(false)
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }

                var hospital = await GetByIdAsync((await _department.GetByIdAsync(bed.DepartmentId)).HospitalId);
                if (hospital.UserId != user.Id)
                {
                    return response.SetMessage("You are not authorize").SetData(false)
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                await _bed.DeleteAsync(bed.Id);
                return await _bed.CompleteAsync() == true ?
                   response.SetData(true).SetMessage("Update successed").SetStatus(StatusCodes.Ok.ToString())
                   : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<IReadOnlyList<BedsReserved>> GetAllBedReservedForHospital(int id)
            => _mapper.Map<IReadOnlyList<ReserveHospital>, IReadOnlyList<BedsReserved>>(await _reserveHospital.GetQuery().Include(e => e.Bed).ThenInclude(e => e.Department).ThenInclude(e => e.Hospital).Where(e => e.Bed.Department.HospitalId == id).ToListAsync());

        public async Task<ResponseService<bool>> CheckReserve(CheckReserveHospital input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbHospital = await base.GetQuery().Where(ex => ex.UserId == user.Id).FirstOrDefaultAsync();
                if (dbHospital == null)
                {
                    return response.SetMessage("You are not hospital").SetData(false).SetStatus(StatusCodes.Unauthorized.ToString());
                }

                var dbReserve = await _reserveHospital.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetMessage("This reserve is not exist").SetData(false).SetStatus(StatusCodes.NotFound.ToString());
                }
                var mapper = _mapper.Map(input, dbReserve);
                var bed = await _bed.GetByIdAsync(dbReserve.BedId);
                var department = await _department.GetByIdAsync(bed.DepartmentId);
                if (department.HospitalId != dbHospital.Id)
                {
                    return response.SetData(false).SetMessage("You are not a hospital for this reserve").SetStatus(StatusCodes.Unauthorized.ToString());
                }
                if (input.ReserveState == ReserveState.Approved)
                {
                    bed.IsAvailable = false;
                    _bed.Update(bed);
                    if (await _reserveHospital.CompleteAsync() == false)
                    {
                        return response.SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetData(false).SetStatus(StatusCodes.InternalServerError.ToString());
                    }
                }
                _reserveHospital.Update(mapper);
                return await _reserveHospital.CompleteAsync() == true ?
                   response.SetData(true).SetMessage("Update successed").SetStatus(StatusCodes.Ok.ToString())
                   : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }
    }
    public interface IHospitalService : IGenericRepository<Hospital>
    {
        public Task<ResponseService<RegisterHospitalOutput>> RegisterHospital(RegisterHospital input);
        public Task<ResponseService<RegisterHospitalOutput>> LoginHospital(LoginHospital input);
        public Task<IReadOnlyList<HospitalOutput>> GetAllHospitals();
        public Task<HospitalOutput> GetHospital(string username);
        public Task<ResponseService<bool>> AddDebartmentsToHospital(List<CreateDepartment> inputs, User user);
        public Task<ResponseService<bool>> UpdateDepartment(UpdateDepartment input, User user);
        public Task<ResponseService<bool>> DeleteDepartment(int id, User user);
        public Task<IReadOnlyList<DepartmentOutput>> GetDepartmentsForHospital(string username);
        public Task<DepartmentOutput> GetDepartment(int id);
        public Task<ResponseService<bool>> AddBedsToDepartment(List<CreateBed> inputs, User user);
        public Task<ResponseService<bool>> UpdateBed(UpdateBed input, User user);
        public Task<ResponseService<bool>> DeleteBed(int id, User user);
        public Task<IReadOnlyList<BedOutput>> GetBedsForDepartment(int departmentId);
        public Task<BedOutput> GetBed(int id);
        public Task<ResponseService<bool>> UpdateHospital(UpdateHospital input, User user);
        public Task<IReadOnlyList<BedsReserved>> GetAllBedReservedForHospital(int id);
        public Task<ResponseService<bool>> CheckReserve(CheckReserveHospital input, User user);
    }
}