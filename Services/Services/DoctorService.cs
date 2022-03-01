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
        private readonly IMapper _mapper;
        private readonly IIdentityRepository _identityRepository;
        private readonly IGenericRepository<DocumentsDoctor> _documentDoctor;
        private readonly ITokenService _tokenService;

        public DoctorService(IIdentityRepository identityRepository, IGenericRepository<DocumentsDoctor> documentDoctor,
         IMapper mapper, ITokenService tokenService, StoreContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
            _identityRepository = identityRepository;
            _documentDoctor = documentDoctor;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<DoctorOutput>> GetAllDoctors()
            => _mapper.Map<IReadOnlyList<Doctor>, IReadOnlyList<DoctorOutput>>(await GetQuery().Include(us => us.User).ToListAsync());

        public async Task<DoctorOutput> GetDoctor(string username)
            => _mapper.Map<Doctor, DoctorOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.UserName == username));

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
                    response.Data = new()
                    {
                        DisplayName = user.FirstName + user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        Token = await _tokenService.CreateToken(user)
                    };
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
                    response.Message = "Time to start work must be less then end time to end work!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                User user = new()
                {
                    UserName = input.UserName,
                    Email = input.Email,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    Gender = (Gender)input.Gender,
                    PhoneNumber = input.PhoneNumber,
                    Location = input.Location,
                    State = (PersonState)input.State,
                    HomeNumber = input.HomeNumber,
                    UserType = UserType.Doctor,
                    City = input.City
                };
                Doctor doctor = new()
                {
                    StartTimeWork = input.StartTimeWork,
                    EndTimeWork = input.EndTimeWork,
                    AboutMe = input.AboutMe,
                    WorkAtHome = input.WorkAtHome,
                    Specialization = input.Specialization,
                    AccountState = AccountState.Pending,
                    UserId = user.Id
                };

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
                    response.Data = new RegisterDoctorOutput()
                    {
                        Id = doctor.Id,
                        DisplayName = input.FirstName + " " + input.LastName,
                        UserName = input.UserName,
                        Email = input.Email,
                        Token = await _tokenService.CreateToken(dbUser)
                    };
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
        public async Task<ResponseService<bool>> UpdateDoctor(UpdateDoctor input, string userId)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var dbDoctor = await GetByIdAsync(input.Id);
                if (dbDoctor == null)
                {
                    response.Message = "This doctor is not exist!";
                    response.Status = StatusCodes.NotFound.ToString();
                    return response;
                }
                var dbUser = await _identityRepository.GetUserByIdAsync(userId);
                if (dbUser.Id != dbDoctor.UserId)
                {
                    response.Message = "You are not authorized";
                    response.Status = StatusCodes.Unauthorized.ToString();
                    return response;
                }

                TimeSpan date = input.EndTimeWork.Subtract(input.StartTimeWork);
                int hours = date.Hours;
                if (input.StartTimeWork >= input.EndTimeWork || hours < 1)
                {
                    response.Message = "Time to start work must be less then end time to end work!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                if (input.City != null)
                    dbUser.City = input.City;
                if (input.FirstName != null)
                    dbUser.FirstName = input.FirstName;
                if (input.LastName != null)
                    dbUser.LastName = input.LastName;
                if (input.PhoneNumber != null)
                    dbUser.PhoneNumber = input.PhoneNumber;
                if (input.Location != null)
                    dbUser.Location = input.Location;
                if (input.HomeNumber != null)
                    dbUser.HomeNumber = input.HomeNumber;
                if (input.State != -1)
                    dbUser.State = (PersonState)input.State;

                if (input.Specialization != null)
                    dbDoctor.Specialization = input.Specialization;
                if (input.AboutMe != null)
                    dbDoctor.AboutMe = input.AboutMe;
                dbDoctor.WorkAtHome = input.WorkAtHome;
                dbDoctor.StartTimeWork = input.StartTimeWork;
                dbDoctor.EndTimeWork = input.EndTimeWork;

                if (await _identityRepository.UpdateUserAsync(dbUser))
                {
                    Update(dbDoctor);
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
    }
    public interface IDoctorService : IGenericRepository<Doctor>
    {
        public Task<IReadOnlyList<DoctorOutput>> GetAllDoctors();
        public Task<DoctorOutput> GetDoctor(string username);
        public Task<ResponseService<LoginOutput>> LoginDoctor(LoginDoctorInput input);
        public Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor(RegisterDoctor input);
        public Task<ResponseService<bool>> UpdateDoctor(UpdateDoctor input, string userId);
    }
}