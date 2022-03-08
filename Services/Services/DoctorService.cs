using System.Data;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Doctor.Inputs;
using Models.Doctor.Outputs;
using Services.Common;

namespace Services
{
    public class DoctorService : GenericRepository<Doctor>, IDoctorService
    { 
        private readonly IIdentityRepository _identityRepository;
        private readonly IGenericRepository<DocumentsDoctor> _documentDoctor;
        private readonly IGenericRepository<ReserveDoctor> _reserveDoctor;
        private readonly ITokenService _tokenService;

        public DoctorService(IIdentityRepository identityRepository, IGenericRepository<DocumentsDoctor> documentDoctor,
         IGenericRepository<ReserveDoctor> reserveDoctor, IMapper mapper, ITokenService tokenService, StoreContext dbContext) : base(dbContext,mapper)
        { 
            _identityRepository = identityRepository;
            _documentDoctor = documentDoctor;
            _reserveDoctor = reserveDoctor;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<DoctorOutput>> GetAllDoctors()
            => _mapper.Map<IReadOnlyList<Doctor>, IReadOnlyList<DoctorOutput>>(await GetQuery().Include(us => us.User).ToListAsync());

        public async Task<DoctorOutput> GetDoctor(string username)
            => _mapper.Map<Doctor, DoctorOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.NormalizedUserName == username.ToUpper()));

        public async Task<ResponseService<LoginOutput>> LoginDoctor(LoginDoctorInput input)
        {
            var response = new ResponseService<LoginOutput>();
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
                var dbDoctor = await GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);

                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                bool found = false;
                foreach (var role in roles)
                {
                    if (role == Roles.Doctor.ToString() || (dbDoctor.AccountState == AccountState.Pending && role == Roles.Sick.ToString()))
                        found = true;
                }
                if (!found)
                {
                    response.Message = "Oooops you are not doctor";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                if (!await _identityRepository.CheckPassword(user, input.Password))
                {
                    response.Message = "Password not correct!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }
                if (await _identityRepository.LoginUser(user, input.Password))
                {
                    response.Message = $"Welcome {user.FirstName + " " + user.LastName}";
                    response.Status = StatusCodes.Ok.ToString();
                    var mapper = _mapper.Map<LoginOutput>(user);
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

        public async Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor(RegisterDoctor input)
        {
            var response = new ResponseService<RegisterDoctorOutput>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                // this user is exist 
                if (await _identityRepository.GetUserByEmailAsync(input.Email) != null || await _identityRepository.GetUserByNameAsync(input.UserName) != null)
                {
                    response.Message = "Username or Email is Exist!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }
                var files = input.Files;

                // doctor register without documents
                if (files.Length == 0)
                {
                    response.Message = "Please send your document if you want to register as a doctor!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                TimeSpan date = input.EndTimeWork.Subtract(input.StartTimeWork);
                int hours = date.Hours;
                if (input.StartTimeWork >= input.EndTimeWork || hours < 1)
                {
                    response.Message = "Start time must be less than end time al least one hour";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                var user = _mapper.Map<User>(input);
                user.UserType = UserType.Doctor;
                var doctor = _mapper.Map<Doctor>(input);
                doctor.UserId = user.Id;
                doctor.AccountState = AccountState.Pending;

                if (await _identityRepository.CreateUserAsync(user, input.Password))
                {
                    await InsertAsync(doctor);
                    if (!await CompleteAsync())
                    {
                        response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                        response.Status = StatusCodes.InternalServerError.ToString();
                        return response;
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine("wwwroot/Documents/doctor/", "DocumentFor" + user.UserName + "_" + file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        await stream.DisposeAsync();
                        var docModel = new DocumentsDoctorModel()
                        {
                            DoctorId = doctor.Id,
                            UrlFile = path[7..]
                        };
                        var doc = _mapper.Map<DocumentsDoctor>(docModel);
                        await _documentDoctor.InsertAsync(doc);
                        if (!await _documentDoctor.CompleteAsync())
                        {
                            response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                            response.Status = StatusCodes.InternalServerError.ToString();
                            return response;
                        }
                    }

                    var dbUser = await _identityRepository.GetUserByEmailAsync(input.Email);
                    await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                    response.Message = $"Welcome {dbUser.FirstName + " " + dbUser.LastName}";
                    response.Status = StatusCodes.Created.ToString();
                    var mapper = _mapper.Map<RegisterDoctorOutput>(user);
                    mapper.Token = await _tokenService.CreateToken(dbUser);
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
        public async Task<ResponseService<bool>> UpdateDoctor(UpdateDoctor input, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var dbDoctor = await GetByIdAsync(input.DoctorId);
                if (dbDoctor == null)
                {
                    response.Message = "This doctor is not exist!";
                    response.Status = StatusCodes.NotFound.ToString();
                    return response;
                }
                if (user.Id != dbDoctor.UserId)
                {
                    response.Message = "You are not authorized";
                    response.Status = StatusCodes.Unauthorized.ToString();
                    return response;
                }

                TimeSpan date = input.EndTimeWork.Subtract(input.StartTimeWork);
                int hours = date.Hours;
                if (input.StartTimeWork >= input.EndTimeWork || hours < 1)
                {
                    response.Message = "Start time must be less than end time al least one hour";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                var userMapper = _mapper.Map(input, user);
                var doctorMapper = _mapper.Map(input, dbDoctor);

                if (await _identityRepository.UpdateUserAsync(userMapper))
                {
                    Update(doctorMapper);
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
        public async Task<IReadOnlyList<ReserveDoctorOutput>> GetAllReversedForDoctor(int id)
            => _mapper.Map<IReadOnlyList<ReserveDoctor>, IReadOnlyList<ReserveDoctorOutput>>(await _reserveDoctor.GetQuery().Where(ex => ex.DoctorId == id).Include(e => e.Doctor).ThenInclude(e => e.User).ToListAsync());
        public async Task<ResponseService<bool>> CheckReserve(CheckReserve input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbDoctor = await base.GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);
                if (dbDoctor == null)
                {
                    return response.SetData(false).SetMessage("You are not a doctor").SetStatus(StatusCodes.Unauthorized.ToString());
                }
                var dbReserve = await _reserveDoctor.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.DoctorId != dbDoctor.Id)
                {
                    return response.SetData(false).SetMessage("You are not a doctor for this reserve").SetStatus(StatusCodes.Unauthorized.ToString());
                }

                var mapper = _mapper.Map(input, dbReserve);
                _reserveDoctor.Update(mapper);

                return await _reserveDoctor.CompleteAsync() == true ?
                response.SetData(true).SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }
    }
    public interface IDoctorService : IGenericRepository<Doctor>
    {
        public Task<IReadOnlyList<DoctorOutput>> GetAllDoctors();
        public Task<DoctorOutput> GetDoctor(string username);
        public Task<ResponseService<LoginOutput>> LoginDoctor(LoginDoctorInput input);
        public Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor(RegisterDoctor input);
        public Task<ResponseService<bool>> UpdateDoctor(UpdateDoctor input, User user);
        public Task<IReadOnlyList<ReserveDoctorOutput>> GetAllReversedForDoctor(int id);
        public Task<ResponseService<bool>> CheckReserve(CheckReserve input, User user);
    }
}