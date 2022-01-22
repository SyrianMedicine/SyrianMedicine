using DAL.DataContext;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        public IDoctorService DoctorServices { get; }

        public UnitOfWork(IDoctorService DoctorServices, StoreContext dbContext)
        {
            this.DoctorServices = DoctorServices;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
    }
}