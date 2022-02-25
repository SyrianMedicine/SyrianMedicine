using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Hospital;
using Models.Hospital.Inputs;
using Models.Hospital.Outputs;
using Services.Common;

namespace Services
{
    public class HospitalService : GenericRepository<Hospital>, IHospitalService
    {
        private readonly IMapper _mapper;
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IGenericRepository<DocumentsHospital> _documentsHospital;

        public HospitalService(IIdentityRepository identityRepository, IGenericRepository<DocumentsHospital> documentsHospital, ITokenService tokenService, IMapper mapper, StoreContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _documentsHospital = documentsHospital;
        }

        public async Task<IReadOnlyList<HospitalOutput>> GetAllHospitals()
            => _mapper.Map<IReadOnlyList<Hospital>, IReadOnlyList<HospitalOutput>>(await GetQuery().Include(e => e.User).ToListAsync());

        public async Task<HospitalOutput> GetHospital(string username)
            => _mapper.Map<Hospital, HospitalOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.UserName == username));

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
                if (await _identityRepository.LoginUser(user, input.Password))
                {
                    response.Message = "Done";
                    response.Status = StatusCodes.Ok.ToString();
                    response.Data = new()
                    {
                        UserName = user.UserName,
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

        public async Task<ResponseService<RegisterHospitalOutput>> RegisterHospital(RegisterHospital input)
        {
            var response = new ResponseService<RegisterHospitalOutput>();
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

                User user = new()
                {
                    UserName = input.UserName,
                    Email = input.Email,
                    PhoneNumber = input.PhoneNumer,
                    Location = input.Location,
                    City = input.City,
                    UserType = UserType.Hospital,
                    HomeNumber = input.HomeNumber,
                    PictureUrl = input.PhoneNumer
                };
                Hospital hospital = new()
                {
                    Name = input.Name,
                    AboutHospital = input.AboutHospital,
                    AccountState = AccountState.Pending,
                    WebSite = input.WebSite,
                    UserId = user.Id
                };

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
                    response.Message = "Done";
                    response.Status = StatusCodes.Created.ToString();
                    response.Data = new RegisterHospitalOutput()
                    {
                        UserName = input.UserName,
                        Token = await _tokenService.CreateToken(dbUser)
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
    }
    public interface IHospitalService : IGenericRepository<Hospital>
    {
        public Task<ResponseService<RegisterHospitalOutput>> RegisterHospital(RegisterHospital input);
        public Task<ResponseService<RegisterHospitalOutput>> LoginHospital(LoginHospital input);
        public Task<IReadOnlyList<HospitalOutput>> GetAllHospitals();
        public Task<HospitalOutput> GetHospital(string username);
    }
}