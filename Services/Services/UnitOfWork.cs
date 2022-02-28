using DAL.DataContext;
using DAL.Repositories;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }
        public ITagService TagService { get; }
        public IUserTagService UserTagService { get;}
        public IIdentityRepository IdentityRepository { get; }

        public UnitOfWork(IUserTagService UserTagService, ITagService TagService, IDoctorService DoctorServices, INurseService NurseServices, ISickService SickServices,
            IIdentityRepository IdentityRepository, IHospitalService HospitalServices, StoreContext dbContext)
        {
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            this.SickServices = SickServices;
            this.HospitalServices = HospitalServices;
            this.IdentityRepository = IdentityRepository;
            this.TagService = TagService;
            this.UserTagService = UserTagService;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }
        public IIdentityRepository IdentityRepository { get; }
        public ITagService TagService { get; }

        public IUserTagService UserTagService { get;}
    }
}