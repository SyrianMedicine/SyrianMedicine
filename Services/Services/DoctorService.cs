using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IReadOnlyList<DoctorsOutput>> GetAllDoctors()
            => _mapper.Map<IReadOnlyList<Doctor>, IReadOnlyList<DoctorsOutput>>(await GetQuery().Include(us => us.User).ToListAsync());

        public async Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor(RegisterDoctor input)
        {
            var response = new ResponseService<RegisterDoctorOutput>();
            try
            {
                // this user is exist 
                if (await _identityRepository.GetUserByEmailAsync(input.Email) != null || await _identityRepository.GetUserByNameAsync(input.UserName) != null)
                {
                    response.Message = "Username or Email is Exist!";
                    response.Data = null;
                    return response;
                }
                var files = input.Files;

                // doctor register without documents
                if (files.Length == 0)
                {
                    response.Message = "Please send your document if you want register as a doctor!";
                    response.Data = null;
                    return response;
                }

                // Doctor doctor = _mapper.Map<RegisterDoctor, Doctor>(input);
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
                        response.Data = null;
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
                            response.Data = null;
                            return response;
                        }
                    }

                    var dbUser = await _identityRepository.GetUserByEmailAsync(input.Email);
                    await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                    response.Message = "Done";
                    response.Data = new RegisterDoctorOutput()
                    {
                        DisplayName = input.FirstName + " " + input.LastName,
                        UserName = input.UserName,
                        Token = await _tokenService.CreateToken(dbUser)
                    };
                }
                else
                {
                    response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                    response.Data = null;
                    return response;
                }
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Data = null;
            }
            return response;
        }
    }
    public interface IDoctorService : IGenericRepository<Doctor>
    {
        public Task<IReadOnlyList<DoctorsOutput>> GetAllDoctors();
        public Task<ResponseService<RegisterDoctorOutput>> RegisterDoctor(RegisterDoctor input);
    }
}