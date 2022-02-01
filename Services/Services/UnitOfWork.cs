using DAL.DataContext;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }

        public UnitOfWork(IDoctorService DoctorServices, INurseService NurseServices, StoreContext dbContext)
        {
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
    }
}