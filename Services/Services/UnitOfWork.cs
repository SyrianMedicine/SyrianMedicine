using DAL.DataContext;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }


        public UnitOfWork(IDoctorService DoctorServices, INurseService NurseServices, ISickService SickServices, StoreContext dbContext)
        {
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            this.SickServices = SickServices;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
    }
}