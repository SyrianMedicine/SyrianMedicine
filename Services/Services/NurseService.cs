using System.Data;
using AutoMapper;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Doctor.Outputs;
using Models.Helper;
using Models.Nurse;
using Models.Nurse.Inputs;
using Models.Nurse.Outputs;
using Services.Common;

namespace Services
{
    public class NurseService : GenericRepository<Nurse>, INurseService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IGenericRepository<DocumentsNurse> _documentNurse;
        private readonly IGenericRepository<ReserveNurse> _reserveNurse;
        private readonly ITokenService _tokenService;
        public NurseService(IIdentityRepository identityRepository, IGenericRepository<DocumentsNurse> documentNurse,
         IMapper mapper, IGenericRepository<ReserveNurse> reserveNurse, ITokenService tokenService, StoreContext dbContext) : base(dbContext, mapper)
        {
            _identityRepository = identityRepository;
            _reserveNurse = reserveNurse;
            _documentNurse = documentNurse;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<NurseOutput>> GetAllNurses()
            => _mapper.Map<IReadOnlyList<Nurse>, IReadOnlyList<NurseOutput>>(await GetQuery().Include(e => e.User).ToListAsync());

        public async Task<NurseOutput> GetNurse(string username)
            => _mapper.Map<Nurse, NurseOutput>(await GetQuery().Include(e => e.User).FirstOrDefaultAsync(e => e.User.NormalizedUserName == username.ToUpper()));


        public async Task<PagedList<NurseOutput>> GetPaginationNurse(NurseQuery input)
        {
            var query = GetQuery().Include(e => e.User).AsQueryable();

            #region Filter
            if (input.WorkAtHome != null)
            {
                query = query.Where(e => e.WorkAtHome == input.WorkAtHome);
            }
            if (input.Gender != null)
            {
                query = query.Where(e => e.User.Gender == (Gender)input.Gender);
            }
            if (!String.IsNullOrEmpty(input.SearchString))
            {
                query = query.Where(e =>
                e.User.Location.ToLower().Contains(input.SearchString.ToLower()) ||
                e.User.FirstName.ToLower().Contains(input.SearchString.ToLower()) ||
                e.User.LastName.ToLower().Contains(input.SearchString.ToLower()) ||
                e.Specialization.ToLower().Contains(input.SearchString.ToLower())
                );
            }
            if (input.StartTimeWork != default(DateTime) || input.EndTimeWork != default(DateTime))
            {
                if (input.StartTimeWork != default(DateTime) && input.EndTimeWork != default(DateTime))
                    query = query.Where(e => e.StartTimeWork.Hour >= input.StartTimeWork.Hour && e.EndTimeWork.Hour <= input.EndTimeWork.Hour);
                else if (input.StartTimeWork != default(DateTime) && input.EndTimeWork != default(DateTime))
                    query = query.Where(e => e.StartTimeWork.Hour >= input.StartTimeWork.Hour);
                else if (input.StartTimeWork != default(DateTime) && input.EndTimeWork != default(DateTime))
                    query = query.Where(e => e.EndTimeWork.Hour >= input.EndTimeWork.Hour);
            }
            #endregion

            if (input.OrderByDesc)
                query = query.OrderByDescending(e => e.Id);
            return _mapper.Map<PagedList<Nurse>, PagedList<NurseOutput>>(await PagedList<Nurse>.CreatePagedListAsync(query, input.OldTotal, input.PageNumber, input.PageSize));
        }


        public async Task<PagedList<MostNursesRated>> GetMostNursesRated(NurseQuery input)
        {
            var query = base.GetQuery().Include(e => e.User).ThenInclude(e => e.UsersRatedMe).OrderByDescending(e => e.User.UsersRatedMe.Average(e => (int)(e.RateValue)));
            return _mapper.Map<PagedList<Nurse>, PagedList<MostNursesRated>>(await PagedList<Nurse>.CreatePagedListAsync(query, input.OldTotal, input.PageNumber, input.PageSize));
        }

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

                var dbNurse = await GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);
                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                bool found = false;
                foreach (var role in roles)
                {
                    if (role == Roles.Nurse.ToString() || (dbNurse.AccountState == AccountState.Pending && role == Roles.Sick.ToString()))
                        found = true;
                }
                if (!found)
                {
                    response.Message = "Oooops you are not nurse";
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
                    var mapper = _mapper.Map<LoginNurseOutput>(user);
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

        public async Task<ResponseService<RegisterNurseOutput>> RegisterNurse(RegisterNurse input)
        {
            var response = new ResponseService<RegisterNurseOutput>();
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

                // nurse register without documents
                if (files.Length == 0)
                {
                    response.Message = "Please send your document if you want register as a nurse!";
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

                var userMapper = _mapper.Map<User>(input);
                userMapper.UserType = UserType.Nurse;
                userMapper.Date = DateTime.UtcNow;
                var nurseMapper = _mapper.Map<Nurse>(input);
                nurseMapper.UserId = userMapper.Id;
                nurseMapper.AccountState = AccountState.Pending;

                if (await _identityRepository.CreateUserAsync(userMapper, input.Password))
                {
                    await InsertAsync(nurseMapper);
                    if (!await CompleteAsync())
                    {
                        response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
                        response.Status = StatusCodes.InternalServerError.ToString();
                        return response;
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine("wwwroot/Documents/nurse/", "DocumentFor" + userMapper.UserName + "_" + file.FileName);
                        var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);
                        await stream.DisposeAsync();
                        var docModel = new DocumentsNurseModel()
                        {
                            NurseId = nurseMapper.Id,
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
                    response.Message = $"Welcome {dbUser.FirstName + " " + dbUser.LastName}";
                    response.Status = StatusCodes.Created.ToString();
                    var mapper = _mapper.Map<RegisterNurseOutput>(dbUser);
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
        public async Task<ResponseService<bool>> UpdateNurse(UpdateNurse input, User user)
        {
            var response = new ResponseService<bool>();
            IDbContextTransaction transaction = await BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var dbNurse = await GetByIdAsync(input.NurseId);
                if (dbNurse == null)
                {
                    response.Message = "This nurse is not exist!";
                    response.Status = StatusCodes.NotFound.ToString();
                    return response;
                }
                if (user.Id != dbNurse.UserId)
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
                var nurseMapper = _mapper.Map(input, dbNurse);

                if (await _identityRepository.UpdateUserAsync(userMapper))
                {
                    Update(nurseMapper);
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

        public async Task<IReadOnlyList<ReserveNurseOutput>> GetAllReversedForNurse(int id)
            => _mapper.Map<IReadOnlyList<ReserveNurse>, IReadOnlyList<ReserveNurseOutput>>(await _reserveNurse.GetQuery().Where(ex => ex.NurseId == id).Include(e => e.Nurse).ThenInclude(e => e.User).ToListAsync());

        public async Task<ResponseService<bool>> CheckReserve(CheckReserve input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbNurse = await base.GetQuery().FirstOrDefaultAsync(ex => ex.UserId == user.Id);
                if (dbNurse == null)
                {
                    return response.SetData(false).SetMessage("You are not a Nurse").SetStatus(StatusCodes.Unauthorized.ToString());
                }
                var dbReserve = await _reserveNurse.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.NurseId != dbNurse.Id)
                {
                    return response.SetData(false).SetMessage("You are not a nurse for this reserve").SetStatus(StatusCodes.Unauthorized.ToString());
                }

                var mapper = _mapper.Map(input, dbReserve);
                _reserveNurse.Update(mapper);

                return await _reserveNurse.CompleteAsync() == true ?
                response.SetData(true).SetMessage("Done").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<PagedList<ReserveNurseData>> GetReserveNurseData(ReserveNurseDataInput input, User nurse)
        {
            var nurseDB = await base.GetQuery().FirstOrDefaultAsync(e => e.UserId == nurse.Id);
            var query = _reserveNurse.GetQuery()
                .Where(e => e.NurseId == nurseDB.Id)
                .Include(e => e.User);
            return _mapper.Map<PagedList<ReserveNurse>, PagedList<ReserveNurseData>>(await PagedList<ReserveNurse>.CreatePagedListAsync(query, 0, input.PageNumber, input.PageSize));
        }

    }
    public interface INurseService : IGenericRepository<Nurse>
    {
        public Task<IReadOnlyList<NurseOutput>> GetAllNurses();
        public Task<NurseOutput> GetNurse(string username);
        public Task<PagedList<NurseOutput>> GetPaginationNurse(NurseQuery input);
        public Task<PagedList<MostNursesRated>> GetMostNursesRated(NurseQuery input);
        public Task<ResponseService<LoginNurseOutput>> LoginNurse(LoginNurseInput input);
        public Task<ResponseService<RegisterNurseOutput>> RegisterNurse(RegisterNurse input);
        public Task<ResponseService<bool>> UpdateNurse(UpdateNurse input, User user);
        public Task<IReadOnlyList<ReserveNurseOutput>> GetAllReversedForNurse(int id);
        public Task<ResponseService<bool>> CheckReserve(CheckReserve input, User user);
        public  Task<PagedList<ReserveNurseData>> GetReserveNurseData(ReserveNurseDataInput input, User nurse);
    }
}