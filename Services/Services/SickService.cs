using AutoMapper;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Models.Sick.Inputs;
using Models.Sick.Outputs;
using Services.Common;

namespace Services
{
    public class SickService : ISickService
    {
        private readonly IMapper _mapper;
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;

        public SickService(IMapper mapper, IIdentityRepository identityRepository, ITokenService tokenService)
        {
            _mapper = mapper;
            _identityRepository = identityRepository;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<SickOutput>> GetAllSicks()
           => _mapper.Map<IReadOnlyList<User>, IReadOnlyList<SickOutput>>(await _identityRepository.AllUsers());

        public async Task<SickOutput> GetSick(string username)
            => _mapper.Map<User, SickOutput>(await _identityRepository.GetUserByNameAsync(username));

        public async Task<ResponseService<LoginSickOutput>> LoginSick(LoginSick input)
        {
            var response = new ResponseService<LoginSickOutput>();
            try
            {
                var user = await _identityRepository.GetUserByNameAsync(input.Username);
                if (user == null)
                {
                    user = await _identityRepository.GetUserByEmailAsync(input.Email);
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
                    if (role == Roles.Sick.ToString())
                        found = true;
                }
                if (!found)
                {
                    response.Message = "Oooops you are not sick";
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
                    response.Message = "Done";
                    response.Status = StatusCodes.Ok.ToString();
                    var mapper = _mapper.Map<User, LoginSickOutput>(user);
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

        public async Task<ResponseService<RegisterSickOutput>> RegisterSick(RegisterSick input)
        {
            var response = new ResponseService<RegisterSickOutput>();
            try
            {
                if (await _identityRepository.GetUserByEmailAsync(input.Email) != null || await _identityRepository.GetUserByNameAsync(input.UserName) != null)
                {
                    response.Message = "Username or Email is exist!";
                    response.Status = StatusCodes.BadRequest.ToString();
                    return response;
                }

                var user = _mapper.Map<RegisterSick, User>(input);
                user.UserType = UserType.Sick;
                if (await _identityRepository.CreateUserAsync(user, input.Password))
                {
                    await _identityRepository.AddRoleToUserAsync(user, Roles.Sick.ToString());
                    response.Message = "Done";
                    response.Status = StatusCodes.Created.ToString();
                    var mapper = _mapper.Map<User, RegisterSickOutput>(user);
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
        public async Task<ResponseService<bool>> UpdateSick(UpdateSick input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var userMapper = _mapper.Map(input, user);
                userMapper.State = (PersonState)input.State;
                userMapper.UserType = UserType.Sick;

                if (await _identityRepository.UpdateUserAsync(userMapper))
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
    }
    public interface ISickService
    {
        public Task<IReadOnlyList<SickOutput>> GetAllSicks();
        public Task<SickOutput> GetSick(string username);
        public Task<ResponseService<LoginSickOutput>> LoginSick(LoginSick input);
        public Task<ResponseService<RegisterSickOutput>> RegisterSick(RegisterSick input);
        public Task<ResponseService<bool>> UpdateSick(UpdateSick input, User user);
    }
}