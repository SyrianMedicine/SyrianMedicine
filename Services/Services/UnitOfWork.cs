using DAL.DataContext;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }

        public IHospitalService HospitalServices { get; }

        public UnitOfWork(IDoctorService DoctorServices, INurseService NurseServices, ISickService SickServices, IHospitalService HospitalServices, StoreContext dbContext)
        {
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            this.SickServices = SickServices;
            this.HospitalServices = HospitalServices;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }

    }
}