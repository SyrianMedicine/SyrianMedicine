using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Nurse;
using Models.Nurse.Inputs;
using Models.Nurse.Outputs;
using Services.Common;

namespace Services
{
    public class NurseService : GenericRepository<Nurse>, INurseService
    {
        private readonly IMapper _mapper;
        private readonly IIdentityRepository _identityRepository;
        private readonly IGenericRepository<DocumentsNurse> _documentNurse;
        private readonly ITokenService _tokenService;
        public NurseService(IIdentityRepository identityRepository, IGenericRepository<DocumentsNurse> documentNurse,
         IMapper mapper, ITokenService tokenService, StoreContext dbContext) : base(dbContext)
        {
            _mapper = mapper;
            _identityRepository = identityRepository;
            _documentNurse = documentNurse;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<NurseOutput>> GetAllNurses()
            => _mapper.Map<IReadOnlyList<Nurse>, IReadOnlyList<NurseOutput>>(await GetQuery().Include(e => e.User).ToListAsync());

        public async Task<NurseOutput> GetNurse(int id)
            => _mapper.Map<Nurse, NurseOutput>(await GetQuery().Where(e => e.Id == id).Include(e => e.User).FirstOrDefaultAsync());

        public async Task<ResponseService<LoginNurseOutput>> LoginNurse(LoginNurseInput input)
        {
            var response = new ResponseService<LoginNurseOutput>();
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
                        FullName = user.FirstName + user.LastName,
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

        public async Task<ResponseService<RegisterNurseOutput>> RegisterNurse(RegisterNurse input)
        {
            var response = new ResponseService<RegisterNurseOutput>();
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

                // nurse register without documents
                if (files.Length == 0)
                {
                    response.Message = "Please send your document if you want register as a nurse!";
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
                    UserType = UserType.Nurse
                };
                Nurse nurse = new()
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
                    await InsertAsync(nurse);
                    if (!await CompleteAsync())
                    {
                        response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                        response.Status = StatusCodes.InternalServerError.ToString();
                        return response;
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine("wwwroot/Documents/nurse/", "DocumentFor" + user.UserName + "_" + file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        await stream.DisposeAsync();
                        var docModel = new DocumentsNurseModel()
                        {
                            NurseId = nurse.Id,
                            UrlFile = path[7..]
                        };
                        var doc = _mapper.Map<DocumentsNurse>(docModel);
                        await _documentNurse.InsertAsync(doc);
                        if (!await _documentNurse.CompleteAsync())
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
                    response.Data = new RegisterNurseOutput()
                    {
                        DisplayName = input.FirstName + " " + input.LastName,
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
    public interface INurseService : IGenericRepository<Nurse>
    {
        public Task<IReadOnlyList<NurseOutput>> GetAllNurses();
        public Task<NurseOutput> GetNurse(int id);
        public Task<ResponseService<LoginNurseOutput>> LoginNurse(LoginNurseInput input);
        public Task<ResponseService<RegisterNurseOutput>> RegisterNurse(RegisterNurse input);
    }
}