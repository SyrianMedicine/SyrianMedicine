using AutoMapper;
using DAL.Entities;
using DAL.Entities.Enums;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Common;
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
        private readonly IGenericRepository<Doctor> _doctor;
        private readonly IGenericRepository<ReserveDoctor> _reserveDoctor;
        private readonly IGenericRepository<Nurse> _nurse;
        private readonly IGenericRepository<ReserveNurse> _reserveNurse;
        private readonly IGenericRepository<Hospital> _hospital;
        private readonly IGenericRepository<Department> _department;
        private readonly IGenericRepository<Bed> _bed;
        private readonly IGenericRepository<ReserveHospital> _reserveHospital;
        private readonly IGenericRepository<HospitalDepartment> _hospitalDepartment;



        public SickService(IMapper mapper, IIdentityRepository identityRepository, IGenericRepository<Nurse> nurse, IGenericRepository<ReserveNurse> reserveNurse,
         IGenericRepository<Doctor> doctor, IGenericRepository<Department> department, IGenericRepository<Bed> bed, IGenericRepository<ReserveHospital> reserveHospital,
         IGenericRepository<Hospital> hospital, IGenericRepository<ReserveDoctor> reserveDoctor, IGenericRepository<HospitalDepartment> hospitalDepartment, ITokenService tokenService)
        {
            _mapper = mapper;
            _identityRepository = identityRepository;
            _doctor = doctor;
            _nurse = nurse;
            _bed = bed;
            _hospital = hospital;
            _department = department;
            _reserveHospital = reserveHospital;
            _reserveNurse = reserveNurse;
            _reserveDoctor = reserveDoctor;
            _hospitalDepartment = hospitalDepartment;
            _tokenService = tokenService;
        }

        public async Task<IReadOnlyList<SickOutput>> GetAllSicks()
           => _mapper.Map<IReadOnlyList<User>, IReadOnlyList<SickOutput>>(await _identityRepository.AllUsers());

        public async Task<SickOutput> GetSick(string username)
            => _mapper.Map<User, SickOutput>(await _identityRepository.GetUserByNameAsync(username));

        // public async Task<ResponseService<LoginSickOutput>> LoginSick(LoginSick input)
        // {
        //     var response = new ResponseService<LoginSickOutput>();
        //     try
        //     {
        //         var user = await _identityRepository.GetUserByNameAsync(input.Username);
        //         if (user == null)
        //         {
        //             user = await _identityRepository.GetUserByEmailAsync(input.Email);
        //             if (user == null)
        //             {
        //                 response.Message = "UserName or Email not exist!";
        //                 response.Status = StatusCodes.NotFound.ToString();
        //                 return response;
        //             }
        //         }
        //         var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
        //         bool found = false;
        //         foreach (var role in roles)
        //         {
        //             if (role == Roles.Sick.ToString())
        //                 found = true;
        //         }
        //         if (!found)
        //         {
        //             response.Message = "Oooops you are not sick";
        //             response.Status = StatusCodes.BadRequest.ToString();
        //             return response;
        //         }
        //         if (!await _identityRepository.CheckPassword(user, input.Password))
        //         {
        //             response.Message = "Password not correct!";
        //             response.Status = StatusCodes.BadRequest.ToString();
        //             return response;
        //         }
        //         if (await _identityRepository.LoginUser(user, input.Password))
        //         {
        //             response.Message = $"Welcome {user.FirstName} {user.LastName}";
        //             response.Status = StatusCodes.Ok.ToString();
        //             var mapper = _mapper.Map<User, LoginSickOutput>(user);
        //             mapper.Token = await _tokenService.CreateToken(user);
        //             response.Data = mapper;
        //         }
        //         else
        //         {
        //             response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown);
        //             response.Status = StatusCodes.InternalServerError.ToString();
        //             return response;
        //         }
        //     }
        //     catch
        //     {
        //         response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
        //         response.Status = StatusCodes.InternalServerError.ToString();
        //     }
        //     return response;
        // }

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
                user.Date = DateTime.UtcNow;
                if (await _identityRepository.CreateUserAsync(user, input.Password))
                {
                    await _identityRepository.AddRoleToUserAsync(user, Roles.Sick.ToString());
                    response.Message = $"Welcome {user.FirstName} {user.LastName}";
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
        public async Task<ResponseService<bool>> ReserveDateWithDoctor(ReserveDateWithDoctor input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                var role = roles.Where(e => e.Equals(Roles.Hospital.ToString())).FirstOrDefault();
                if (role != null)
                {
                    return response.SetData(false).SetMessage("Hospital can't reserve date with doctor")
                                   .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var dbDoctor = await _doctor.GetQuery().Where(ex => ex.Id == input.DoctorId).Include(e => e.User).FirstOrDefaultAsync();
                if (dbDoctor == null)
                {
                    return response.SetData(false).SetMessage("This doctor is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbDoctor.UserId == user.Id)
                {
                    return response.SetData(false).SetMessage("Loool you can't reserve date with yourself")
                                   .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var dbReserve = await _reserveDoctor.GetQuery().Include(e => e.Doctor).Where(e => e.UserId == user.Id && e.DoctorId == input.DoctorId).FirstOrDefaultAsync();
                if (dbReserve != null)
                {
                    return response.SetData(false).SetMessage("You can't reserve more than one date")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }
                if (dbDoctor.AccountState == AccountState.Pending)
                {
                    return response.SetData(false).SetMessage("This Doctor is not approved yet")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var reserve = _mapper.Map<ReserveDoctor>(input);
                reserve.UserId = user.Id;
                reserve.ReserveState = ReserveState.Pending;
                reserve.DateTime = DateTime.UtcNow;
                await _reserveDoctor.InsertAsync(reserve);
                return await _reserveDoctor.CompleteAsync() == true ?
                    response.SetData(true).SetMessage($"Your date is waiting doctor {dbDoctor.User.UserName} to approve it")
                            .SetStatus(StatusCodes.Created.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                              .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }
        public async Task<ResponseService<bool>> UpdateReserveDateWithDoctor(UpdateReserveDateWithDoctor input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveDoctor.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }

                var mapper = _mapper.Map(input, dbReserve);
                if (mapper.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("This reserve is not for you!")
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }
                _reserveDoctor.Update(mapper);
                return await _reserveDoctor.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Reverse updated")
                            .SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> DeleteReserveDateWithDoctor(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {

                var dbReserve = await _reserveDoctor.GetByIdAsync(id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("This reserve is not for you!")
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                await _reserveDoctor.DeleteAsync(dbReserve.Id);
                return await _reserveDoctor.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Reverse deleted")
                            .SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> ReserveDateWithNurse(ReserveDateWithNurse input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var roles = await _identityRepository.GetRolesByUserIdAsync(user.Id);
                var role = roles.Where(e => e.Equals(Roles.Hospital.ToString())).FirstOrDefault();
                if (role != null)
                {
                    return response.SetData(false).SetMessage("Hospital can't reserve date with doctor")
                                   .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var dbNurse = await _nurse.GetQuery().Where(ex => ex.Id == input.NurseId).Include(e => e.User).FirstOrDefaultAsync();
                if (dbNurse == null)
                {
                    return response.SetData(false).SetMessage("This nurse is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbNurse.UserId == user.Id)
                {
                    return response.SetData(false).SetMessage("Loool you can't reserve date with yourself")
                                   .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var dbReserve = await _reserveNurse.GetQuery().Include(e => e.Nurse).Where(e => e.UserId == user.Id && e.NurseId == input.NurseId).FirstOrDefaultAsync();
                if (dbReserve != null)
                {
                    return response.SetData(false).SetMessage("You can't reserve more than one date")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }

                if (dbNurse.AccountState == AccountState.Pending)
                {
                    return response.SetData(false).SetMessage("This Nurse is not approved yet")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var reserve = _mapper.Map<ReserveNurse>(input);
                reserve.UserId = user.Id;
                reserve.ReserveState = ReserveState.Pending;
                reserve.DateTime = DateTime.UtcNow;
                await _reserveNurse.InsertAsync(reserve);
                return await _reserveNurse.CompleteAsync() == true ?
                    response.SetData(true).SetMessage($"Your date is waiting nurse {dbNurse.User.UserName} to approve it")
                            .SetStatus(StatusCodes.Created.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                              .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> UpdateReserveDateWithNurse(UpdateReserveDateWithNurse input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveNurse.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }

                var mapper = _mapper.Map(input, dbReserve);
                if (mapper.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("This reserve is not for you!")
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }
                _reserveNurse.Update(mapper);
                return await _reserveNurse.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Reverse updated")
                            .SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> DeleteReserveDateWithNurse(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveNurse.GetByIdAsync(id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("This reserve is not for you!")
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                await _reserveNurse.DeleteAsync(dbReserve.Id);
                return await _reserveNurse.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Reverse deleted")
                            .SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> ReserveBedInHospital(ReserveBedInHospital input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbHospital = await _hospital.GetByIdAsync(input.HospitalId);
                if (dbHospital == null)
                {
                    return response.SetData(false).SetMessage("This hospital is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbHospital.AccountState == AccountState.Pending)
                {
                    return response.SetData(false).SetMessage("This Hospital is not approved yet")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }
                var departmentDb = await _department.GetByIdAsync(input.DepartmentId);
                if (departmentDb == null)
                {
                    return response.SetData(false).SetMessage("This department is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }

                var dbBeds = await _bed.GetQuery().Where(e => e.DepartmentId == departmentDb.Id && e.HospitalId == dbHospital.Id).ToListAsync();
                if (dbBeds == null)
                {
                    return response.SetData(false).SetMessage("This department not has beds").SetStatus(StatusCodes.NotFound.ToString());
                }


                var hospitalDepartment = await _hospitalDepartment.GetQuery().Where(e => e.DepartmentId == departmentDb.Id && e.HospitalId == dbHospital.Id).FirstOrDefaultAsync();
                if (hospitalDepartment == null)
                {
                    return response.SetData(false).SetMessage("you are not authorize!!").SetStatus(StatusCodes.Unauthorized.ToString());
                }

                Bed availableBed = null;
                foreach (var bed in dbBeds)
                {
                    if (bed.IsAvailable)
                    {
                        availableBed = bed;

                    }
                }
                if (availableBed == null)
                {
                    return response.SetData(false).SetMessage("This Department is busy now").SetStatus(StatusCodes.BadRequest.ToString());
                }


                var dbReserves = await _reserveHospital.GetQuery().Where(e => e.UserId == user.Id).Include(e => e.Bed).ToListAsync();
                bool foundReserve = false;

                foreach (var dbreserve in dbReserves)
                {
                    if (dbreserve.Bed.HospitalId == input.HospitalId)
                        foundReserve = true;

                }
                if (foundReserve)
                {
                    return response.SetData(false).SetMessage("You can't reserve more than one date")
                                    .SetStatus(StatusCodes.BadRequest.ToString());
                }

                var reserve = _mapper.Map<ReserveHospital>(input);
                reserve.UserId = user.Id;
                reserve.ReserveState = ReserveState.Pending;
                reserve.DateTime = DateTime.UtcNow;
                reserve.BedId = availableBed.Id;
                await _reserveHospital.InsertAsync(reserve);
                availableBed.IsAvailable = false;
                _bed.Update(availableBed);
                return await _reserveHospital.CompleteAsync() == true ?
                    response.SetData(true).SetMessage($"Your date is waiting hospital {dbHospital.Name} to approve it")
                            .SetStatus(StatusCodes.Created.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }

        public async Task<ResponseService<bool>> UpdateReserveBedInHospital(UpdateReserveBedInHospital input, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveHospital.GetByIdAsync(input.Id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                var mapper = _mapper.Map(input, dbReserve);
                _reserveHospital.Update(mapper);
                return await _reserveHospital.CompleteAsync() == true ?
                    response.SetData(true).SetMessage($"reserve updated")
                            .SetStatus(StatusCodes.Created.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                              .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }
        public async Task<ResponseService<bool>> DeleteReserveBedInHospital(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbReserve = await _reserveHospital.GetByIdAsync(id);
                if (dbReserve == null)
                {
                    return response.SetData(false).SetMessage("This reserve is not exist")
                                   .SetStatus(StatusCodes.NotFound.ToString());
                }
                if (dbReserve.UserId != user.Id)
                {
                    return response.SetData(false).SetMessage("This reserve is not for you!")
                                   .SetStatus(StatusCodes.Unauthorized.ToString());
                }

                await _reserveHospital.DeleteAsync(dbReserve.Id);
                return await _reserveHospital.CompleteAsync() == true ?
                    response.SetData(true).SetMessage("Reverse deleted")
                            .SetStatus(StatusCodes.Ok.ToString())
                    : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown))
                            .SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                response.Message = ErrorMessageService.GetErrorMessage(ErrorMessage.InternalServerError);
                response.Status = StatusCodes.InternalServerError.ToString();
                return response;
            }
        }
        public List<OptionDto> GetReserveTypes()
            => Enum.GetValues<ReserveTypes>().Cast<ReserveTypes>().Select(e => new OptionDto { Id = (int)e, Name = e.ToString() }).ToList();



    }
    public interface ISickService
    {
        public Task<IReadOnlyList<SickOutput>> GetAllSicks();
        public Task<SickOutput> GetSick(string username);
        // public Task<ResponseService<LoginSickOutput>> LoginSick(LoginSick input);
        public Task<ResponseService<RegisterSickOutput>> RegisterSick(RegisterSick input);
        public Task<ResponseService<bool>> UpdateSick(UpdateSick input, User user);
        public Task<ResponseService<bool>> ReserveDateWithDoctor(ReserveDateWithDoctor input, User user);
        public Task<ResponseService<bool>> UpdateReserveDateWithDoctor(UpdateReserveDateWithDoctor input, User user);
        public Task<ResponseService<bool>> DeleteReserveDateWithDoctor(int id, User user);
        public Task<ResponseService<bool>> ReserveDateWithNurse(ReserveDateWithNurse input, User user);
        public Task<ResponseService<bool>> UpdateReserveDateWithNurse(UpdateReserveDateWithNurse input, User user);
        public Task<ResponseService<bool>> DeleteReserveDateWithNurse(int id, User user);
        public Task<ResponseService<bool>> ReserveBedInHospital(ReserveBedInHospital input, User user);
        public Task<ResponseService<bool>> UpdateReserveBedInHospital(UpdateReserveBedInHospital input, User user);
        public Task<ResponseService<bool>> DeleteReserveBedInHospital(int id, User user);
        public List<OptionDto> GetReserveTypes();
    }
}