using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Admin.Inputs;
using Models.Admin.Outputs;
using Models.Common;
using Models.Helper;
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

        public async Task<PagedList<OptionDto>> GetCities(Pagination input)
            => _mapper.Map<PagedList<City>, PagedList<OptionDto>>(await PagedList<City>.CreatePagedListAsync(_cityRepository.GetQuery(), input.PageNumber, input.PageSize));


        public List<OptionDto> GetAccountStates()
            => Enum.GetValues<AccountState>().Cast<AccountState>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public async Task<IReadOnlyList<OptionDto>> GetCities()
            => _mapper.Map<IReadOnlyList<City>, IReadOnlyList<OptionDto>>(await _cityRepository.GetAllAsync());

        public List<OptionDto> GetGenders()
            => Enum.GetValues<Gender>().Cast<Gender>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

        public List<OptionDto> GetRoles()
            => Enum.GetValues<Roles>().Cast<Roles>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();
        public List<OptionDto> ReserveStates()
            => Enum.GetValues<ReserveState>().Cast<ReserveState>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();
        public List<OptionDto> GetUserTypes()
            => Enum.GetValues<UserType>().Cast<UserType>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();
        public List<OptionDto> GetPersonStates()
            => Enum.GetValues<PersonState>().Cast<PersonState>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();

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
                    var mapper = _mapper.Map<LoginAdminOutput>(user);
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

        public async Task<ResponseService<bool>> UploadImage(UploadImage input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                if (File.Exists("wwwroot/" + user.PictureUrl))
                    File.Delete("wwwroot/" + user.PictureUrl);

                var path = Path.Combine("wwwroot/users_images/" + "ProfileImageFor_" + user.UserName + "_" + input.imageUrl.FileName);
                var stream = new FileStream(path, FileMode.Create);
                await input.imageUrl.CopyToAsync(stream);
                await stream.DisposeAsync();
                user.PictureUrl = path[7..];
                await _identityRepository.UpdateUserAsync(user);
                return response.SetData(true).SetMessage("Image updated").SetStatus(StatusCodes.Ok.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> UpdateAdminProfile(UpdateAdmin input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {

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

                var mapper = _mapper.Map(input, user);
                if (await _identityRepository.UpdateUserAsync(mapper))
                {
                    response.Data = true;
                    response.Message = "Update successed";
                    response.Status = StatusCodes.Ok.ToString();
                }
                else
                {
                    response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                    response.Status = StatusCodes.InternalServerError.ToString();
                }
                return response;
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
            }
            return response;
        }

        public async Task<bool> IsUserNameExist(string username)
            => await _identityRepository.GetUserByNameAsync(username) != null;
        public async Task<bool> IsEmailExist(string email)
            => await _identityRepository.GetUserByEmailAsync(email) != null;
        public async Task<string> GetUserType(string username)
            => (await _identityRepository.GetUserByNameAsync(username)).UserType.ToString();

    }
    public interface IAccountService
    {
        public Task<bool> IsUserNameExist(string username);
        public Task<bool> IsEmailExist(string email);
        public Task<PagedList<OptionDto>> GetCities(Pagination input);
        public Task<IReadOnlyList<OptionDto>> GetCities();
        public List<OptionDto> GetGenders();
        public List<OptionDto> GetUserTypes();
        public List<OptionDto> GetAccountStates();
        public List<OptionDto> GetRoles();
        public List<OptionDto> ReserveStates();
        public List<OptionDto> GetPersonStates();
        public Task<ResponseService<bool>> UploadImage(UploadImage input, User user);
        public Task<ResponseService<LoginAdminOutput>> LoginAdmin(LoginInput input);
        public Task<ResponseService<bool>> UpdateAdminProfile(UpdateAdmin input, User user);
        public Task<string> GetUserType(string username);
    }
}