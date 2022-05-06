using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Services.Common;

namespace Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IGenericRepository<Hospital> _hospital;
        private readonly IGenericRepository<Doctor> _doctor;
        private readonly IGenericRepository<Nurse> _nurse;
        private readonly IIdentityRepository _identityRepository;
        public DashboardService(IGenericRepository<Hospital> hospital, IGenericRepository<Doctor> doctor, IGenericRepository<Nurse> nurse, IIdentityRepository identityRepository)
        {
            _nurse = nurse; ;
            _doctor = doctor;
            _hospital = hospital;
            _identityRepository = identityRepository;
        }
        public async Task<ResponseService<bool>> ValidateDoctor(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbDoctor = await _doctor.GetQuery().Where(e => e.Id == id).Include(e => e.User).FirstOrDefaultAsync();
                if (dbDoctor == null)
                {
                    return response.SetData(false).SetMessage("This Doctor is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                dbDoctor.AccountState = AccountState.Approved;
                _doctor.Update(dbDoctor);
                return await _doctor.CompleteAsync() == true ?
                response.SetData(true).SetMessage($"Doctor. {dbDoctor.User.FirstName + dbDoctor.User.LastName} is Approved").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> ValidateNurse(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbNurse = await _nurse.GetQuery().Where(e => e.Id == id).Include(e => e.User).FirstOrDefaultAsync();
                if (dbNurse == null)
                {
                    return response.SetData(false).SetMessage("This Nurse is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                dbNurse.AccountState = AccountState.Approved;
                _nurse.Update(dbNurse);
                return await _nurse.CompleteAsync() == true ?
                response.SetData(true).SetMessage($"Nurse. {dbNurse.User.FirstName + dbNurse.User.LastName} is Approved").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> ValidateHospital(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbHospital = await _hospital.GetByIdAsync(id);
                if (dbHospital == null)
                {
                    return response.SetData(false).SetMessage("This Hospital is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                dbHospital.AccountState = AccountState.Approved;
                _hospital.Update(dbHospital);
                return await _hospital.CompleteAsync() == true ?
                response.SetData(true).SetMessage($"Hospital. {dbHospital.Name} is Approved").SetStatus(StatusCodes.Ok.ToString())
                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> RejectHospital(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbHospital = await _hospital.GetByIdAsync(id);
                if (dbHospital == null)
                {
                    return response.SetData(false).SetMessage("This Hospital is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                var dbUser = await _identityRepository.GetUserByIdAsync(dbHospital.UserId);
                await _hospital.DeleteAsync(dbHospital.Id);
                var result = await _hospital.CompleteAsync();
                dbUser.UserType = UserType.Sick;
                await _identityRepository.UpdateUserAsync(dbUser);
                await _identityRepository.DeleteRoleInUser(dbUser, Roles.Hospital.ToString());
                await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                return result == true ?
                                response.SetData(true).SetMessage($"Hospital. {dbHospital.Name} is Approved").SetStatus(StatusCodes.Ok.ToString())
                                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch (Exception ex)
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> RejectNurse(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbNurse = await _nurse.GetByIdAsync(id);
                if (dbNurse == null)
                {
                    return response.SetData(false).SetMessage("This Nurse is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                var dbUser = await _identityRepository.GetUserByIdAsync(dbNurse.UserId);
                await _nurse.DeleteAsync(dbNurse.Id);
                var result = await _nurse.CompleteAsync();
                dbUser.UserType = UserType.Sick;
                await _identityRepository.UpdateUserAsync(dbUser);
                await _identityRepository.DeleteRoleInUser(dbUser, Roles.Nurse.ToString());
                await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                return result == true ?
                                response.SetData(true).SetMessage("This Nurse is Rejected").SetStatus(StatusCodes.Ok.ToString())
                               : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch (Exception ex)
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

        public async Task<ResponseService<bool>> RejectDoctor(int id, User user)
        {
            var response = new ResponseService<bool>();
            try
            {
                var dbDoctor = await _doctor.GetByIdAsync(id);
                if (dbDoctor == null)
                {
                    return response.SetData(false).SetMessage("This Doctor is not exist").SetStatus(StatusCodes.NotFound.ToString());
                }
                var dbUser = await _identityRepository.GetUserByIdAsync(dbDoctor.UserId);
                await _doctor.DeleteAsync(dbDoctor.Id);
                var result = await _doctor.CompleteAsync();
                dbUser.UserType = UserType.Sick;
                await _identityRepository.UpdateUserAsync(dbUser);
                await _identityRepository.DeleteRoleInUser(dbUser, Roles.Doctor.ToString());
                await _identityRepository.AddRoleToUserAsync(dbUser, Roles.Sick.ToString());
                return result == true ?
                                response.SetData(true).SetMessage("This Doctor is Rejected").SetStatus(StatusCodes.Ok.ToString())
                                : response.SetData(false).SetMessage(ErrorMessageService.GetErrorMessage(ErrorMessage.UnKnown)).SetStatus(StatusCodes.InternalServerError.ToString());
            }
            catch (Exception ex)
            {
                return ResponseService<bool>.GetExeptionResponse();
            }
        }

    }
    public interface IDashboardService
    {
        public Task<ResponseService<bool>> ValidateHospital(int id, User user);
        public Task<ResponseService<bool>> ValidateNurse(int id, User user);
        public Task<ResponseService<bool>> ValidateDoctor(int id, User user);

        public Task<ResponseService<bool>> RejectHospital(int id, User user);
        public Task<ResponseService<bool>> RejectNurse(int id, User user);
        public Task<ResponseService<bool>> RejectDoctor(int id, User user);
    }
}