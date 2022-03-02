using DAL.DataContext;
using DAL.Repositories;
using Services.Services;

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
        public IUserTagService UserTagService { get; }
        public IFollowService FollowService { get; }
        public IIdentityRepository IdentityRepository { get; }

        public IAccountService AccountService { get; }

        public UnitOfWork(IFollowService FollowService, IUserTagService UserTagService, ITagService TagService, IDoctorService DoctorServices, INurseService NurseServices, ISickService SickServices,
            IIdentityRepository IdentityRepository, IAccountService AccountService, IHospitalService HospitalServices, StoreContext dbContext)
        {
            this.FollowService = FollowService;
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            this.SickServices = SickServices;
            this.HospitalServices = HospitalServices;
            this.IdentityRepository = IdentityRepository;
            this.TagService = TagService;
            this.UserTagService = UserTagService;
            this.AccountService = AccountService;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }
        public ITagService TagService { get; }
        public IUserTagService UserTagService { get; }
        public IFollowService FollowService { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IAccountService AccountService { get; }
    }
}