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
using Models.Helper;
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
        private readonly IGenericRepository<HospitalHistory> _historyHospital;
        private readonly IGenericRepository<HospitalDepartment> _hospitalDepartment;

        public HospitalService(IIdentityRepository identityRepository, IGenericRepository<DocumentsHospital> documentsHospital,
        IGenericRepository<Department> department, IGenericRepository<HospitalDepartment> hospitalDepartment, IGenericRepository<HospitalHistory> historyHospital,
        IGenericRepository<ReserveHospital> reserveHospital, IGenericRepository<Bed> bed, ITokenService tokenService, IMapper mapper, StoreContext dbContext) : base(dbContext, mapper)
        {
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _documentsHospital = documentsHospital;
            _reserveHospital = reserveHospital;
            _department = department;
            _historyHospital = historyHospital;
            _hospitalDepartment = hospitalDepartment;
            _bed = bed;
        }

        public async Task<IReadOnlyList<HospitalOutput>> GetAllHospitals()
            => _mapper.Map<IReadOnlyList<Hospital>, IReadOnlyList<HospitalOutput>>(await GetQuery().Include(e => e.User).ToListAsync());

        public async Task<HospitalOutput> GetHospital(string username)
            => _mapper.Map<Hospital, HospitalOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.NormalizedUserName == username.ToUpper()));

        public async Task<PagedList<HospitalOutput>> GetPaginationHospital(HospitalQuery input)
        {
            var query = GetQuery().Include(e => e.User)
                                  .Include(e => e.HospitalsDepartments)
                                  .ThenInclude(e => e.Department)
                                  .AsQueryable();

            #region  Filter
            if (!String.IsNullOrEmpty(input.SearchString))
            {
                query = query.Where(e =>
                    e.User.City.ToLower().Contains(input.SearchString.ToLower()) ||
                    e.Name.ToLower().Contains(input.SearchString.ToLower())
                );
            }


            if (!String.IsNullOrEmpty(input.DepartmentName))
            {
                if (input.HasAvilableBed != null)
                {
                    query = query.Where(e =>
                        e.HospitalsDepartments
                            .Any(d =>
                                d.Department.Name.ToLower()
                                    .Contains(input.DepartmentName.ToLower()) &&
                                d.Department.Beds
                                    .Any(b => b.IsAvailable == input.HasAvilableBed)))
                    .AsQueryable();
                }
                else
                {
                    query = query.Where(e =>
                        e.HospitalsDepartments
                            .Any(d =>
                                 d.Department.Name.ToLower()
                                    .Contains(input.DepartmentName.ToLower())))
                    .AsQueryable();

                }
            }
            if (input.HasAvilableBed != null)
            {
                query = query.Where(e => e.HospitalsDepartments.Any(d => d.Department.Beds.Any(b => b.IsAvailable == input.HasAvilableBed)));
            }
            #endregion

            if (input.OrderByDesc)
                query = query.OrderByDescending(e => e.Id);

            return _mapper.Map<PagedList<Hospital>, PagedList<HospitalOutput>>(await PagedList<Hospital>.CreatePagedListAsync(query, input.OldTotal, input.PageNumber, input.PageSize));
        }

        public async Task<PagedList<MostHospitalsRated>> GetMostHospitalsRated(HospitalQuery input)
        {
            var query = base.GetQuery().Include(e => e.User).ThenInclude(e => e.UsersRatedMe).OrderByDescending(e => e.User.UsersRatedMe.Average(e => (int)(e.RateValue)));
            return _mapper.Map<PagedList<Hospital>, PagedList<MostHospitalsRated>>(await PagedList<Hospital>.CreatePagedListAsync(query, input.OldTotal, input.PageNumber, input.PageSize));
        }
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
                    var hospital = await base.GetQuery().Where(e => e.UserId == user.Id).FirstOrDefaultAsync();
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
                    HospitalDepartment obj = new();
                    obj.Hospital = hospital;
                    obj.Department = department;
                    await _hospitalDepartment.InsertAsync(obj);
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
            var hospitalsDepartments = await _hospitalDepartment.GetQuery().Where(e => e.HospitalId == hospital.Id).ToListAsync();
            List<Department> departments = new();
            foreach (var item in hospitalsDepartments)
            {
                departments.Add(await _department.GetByIdAsync(item.DepartmentId));
            }

            return _mapper.Map<IReadOnlyList<Department>, IReadOnlyList<DepartmentOutput>>(departments);
        }
        public async Task<DepartmentOutput> GetDepartment(int id)
            => _mapper.Map<Department, DepartmentOutput>(await _department.GetQuery().FirstOrDefaultAsync(e => e.Id == id));

        public async Task<ResponseService<bool>> AddBedsToDepartment(List<CreateBed> inputs, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                foreach (var input in inputs)
                {
                    var hospital = await base.GetQuery().Where(e => e.HospitalsDepartments.FirstOrDefault(ex => ex.DepartmentId == input.DepartmentId).DepartmentId == input.DepartmentId).FirstOrDefaultAsync();
                    if (hospital.UserId != user.Id)
                    {
                        return response.SetMessage("You are not authorize").SetData(false).SetStatus(StatusCodes.Unauthorized.ToString());
                    }
                    var mapper = _mapper.Map<CreateBed, Bed>(input);
                    mapper.HospitalId = hospital.Id;
                    await _bed.InsertAsync(mapper);
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
                var dbDepartment = await _department.GetQuery().FirstOrDefaultAsync(e => e.Id == input.Id);
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
                var dbDepartmentHospitalId = await _hospitalDepartment.GetQuery().Where(e => e.DepartmentId == dbDepartment.Id && e.HospitalId == dbHospital.Id).FirstOrDefaultAsync();
                if (dbDepartmentHospitalId == null)
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
                var dbDepartment = await _department.GetQuery().FirstOrDefaultAsync(e => e.Id == id);
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
                var dbDepartmentHospitalId = await _hospitalDepartment.GetQuery().Where(e => e.DepartmentId == dbDepartment.Id && e.HospitalId == dbHospital.Id).FirstOrDefaultAsync();
                if (dbDepartmentHospitalId == null)
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

                var hospital = await base.GetQuery().Where(e => e.HospitalsDepartments.FirstOrDefault(ex => ex.DepartmentId == input.DepartmentId).DepartmentId == input.DepartmentId).FirstOrDefaultAsync();
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

                var hospital = await base.GetQuery().Where(e => e.HospitalsDepartments.FirstOrDefault(ex => ex.DepartmentId == bed.DepartmentId).DepartmentId == bed.DepartmentId).FirstOrDefaultAsync();
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


        public async Task<ResponseService<bool>> CheckReserve(CheckReserveHospital input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbHospital = await base.GetQuery().Where(ex => ex.UserId == user.Id).FirstOrDefaultAsync();
                if (dbHospital == null)
                {
                    return response.SetMessage("You are not a hospital").SetData(false).SetStatus(StatusCodes.Unauthorized.ToString());
                }

                var dbReserve = await _reserveHospital.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetMessage("This reserve is not exist").SetData(false).SetStatus(StatusCodes.NotFound.ToString());
                }

                var reserveHospital = await _reserveHospital.GetQuery().Include(e => e.Bed).ThenInclude(e => e.Hospital).Where(e => e.Bed.HospitalId == dbHospital.Id).FirstOrDefaultAsync();
                if (reserveHospital == null)
                {
                    return response.SetData(false).SetMessage("You are not a hospital for this reserve").SetStatus(StatusCodes.Unauthorized.ToString());
                }


                var mapper = _mapper.Map(input, dbReserve);
                var bed = await _bed.GetByIdAsync(dbReserve.BedId);

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
        public async Task<ResponseService<bool>> MoveReserveToHistory(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveHospital.GetByIdAsync(id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("Not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("you are unauthorize").SetStatus(StatusCodes.Unauthorized.ToString());
                }

                HospitalHistory history = new()
                {
                    BedId = dbReserve.BedId,
                    DateTime = dbReserve.DateTime,
                    Description = dbReserve.Description,
                    Title = dbReserve.Title,
                    UserId = dbReserve.UserId
                };
                await _reserveHospital.DeleteAsync(dbReserve);
                await _historyHospital.InsertAsync(history);
                await _historyHospital.CompleteAsync();
                return await _reserveHospital.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Moved successed").SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.BadRequest.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<IReadOnlyList<BedOutput>> GetBedsForHospital(BedForHospitalInput input)
        {
            var beds = await _bed.GetQuery().Where(e => e.DepartmentId == input.DepartmentId && e.HospitalId == input.HospitalId).Include(e => e.Department).ToListAsync();
            return _mapper.Map<IReadOnlyList<Bed>, IReadOnlyList<BedOutput>>(beds);
        }

    }
    public interface IHospitalService : IGenericRepository<Hospital>
    {
        public Task<ResponseService<RegisterHospitalOutput>> RegisterHospital(RegisterHospital input);
        public Task<ResponseService<RegisterHospitalOutput>> LoginHospital(LoginHospital input);
        public Task<IReadOnlyList<HospitalOutput>> GetAllHospitals();
        public Task<PagedList<HospitalOutput>> GetPaginationHospital(HospitalQuery input);
        public Task<PagedList<MostHospitalsRated>> GetMostHospitalsRated(HospitalQuery input);
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
        public Task<ResponseService<bool>> CheckReserve(CheckReserveHospital input, User user);
        public Task<ResponseService<bool>> MoveReserveToHistory(int id, User user);
        public Task<IReadOnlyList<BedOutput>> GetBedsForHospital(BedForHospitalInput input);
    }
}