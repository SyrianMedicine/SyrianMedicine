using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Models.Admin.Inputs;
using Models.Admin.Outputs;
using Models.Common;
using Models.User;
using Services.Common;

namespace Services
{
    public class AccountService : IAccountService
    {
        private readonly IGenericRepository<City> _cityRepository;
        private readonly IMapper _mapper;
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        public AccountService(IGenericRepository<City> cityRepository, ITokenService tokenService, IIdentityRepository identityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public List<OptionDto> GetAccountStates()
            => Enum.GetValues<AccountState>().Cast<AccountState>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public async Task<IReadOnlyList<OptionDto>> GetCities()
            => _mapper.Map<IReadOnlyList<City>, IReadOnlyList<OptionDto>>(await _cityRepository.GetAllAsync());

        public List<OptionDto> GetGenders()
            => Enum.GetValues<Gender>().Cast<Gender>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public List<OptionDto> GetRoles()
            => Enum.GetValues<Roles>().Cast<Roles>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public List<OptionDto> GetUserTypes()
            => Enum.GetValues<UserType>().Cast<UserType>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public async Task<ResponseService<LoginAdminOutput>> LoginAdmin(LoginInput input)
        {
            var response = new ResponseService<LoginAdminOutput>();
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
                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                bool found = false;
                foreach (var role in roles)
                {
                    if (role == Roles.Admin.ToString())
                        found = true;
                }
                if (!found)
                {
                    response.Message = "Oooops you are not admin";
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

        public async Task<ResponseService<bool>> UploadImage(UploadImage input, string userId)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbUser = await _identityRepository.GetUserByIdAsync(userId);
                if (File.Exists("wwwroot/" + dbUser.PictureUrl))
                    File.Delete("wwwroot/" + dbUser.PictureUrl);

                var path = Path.Combine("wwwroot/Users/Images/" + "ProfileImageFor_" + dbUser.UserName + "_" + input.imageUrl.FileName);
                var stream = new FileStream(path, FileMode.Create);
                await input.imageUrl.CopyToAsync(stream);
                await stream.DisposeAsync();
                dbUser.PictureUrl = path[7..];
                await _identityRepository.UpdateUserAsync(dbUser);
                return response.SetData(true).SetMessage("Image updated").SetStatus(StatusCodes.Ok.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }
    }
    public interface IAccountService
    {
        public Task<IReadOnlyList<OptionDto>> GetCities();
        public List<OptionDto> GetGenders();
        public List<OptionDto> GetUserTypes();
        public List<OptionDto> GetAccountStates();
        public List<OptionDto> GetRoles();
        public Task<ResponseService<bool>> UploadImage(UploadImage input, string userId);
        public Task<ResponseService<LoginAdminOutput>> LoginAdmin(LoginInput input);
    }
}