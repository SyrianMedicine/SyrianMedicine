using DAL.DataContext;
using DAL.Repositories;
using Services.Services;

namespace Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;

        public ICommentService CommentService { get; }
        public IPostService PostService { get; }
        public IConnectionService ConnectionService { get; }
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }
        public ITagService TagService { get; }
        public IUserTagService UserTagService { get; }
        public IFollowService FollowService { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IAccountService AccountService { get; }
        public IDashboardService DashboardService { get; }

        public IRatingService RatingService { get; }
        public ILikeService LikeService { get; }

        public UnitOfWork(ILikeService LikeService, IRatingService RatingService, ICommentService CommentService, IPostService PostService, IConnectionService ConnectionService, IFollowService FollowService, IUserTagService UserTagService, ITagService TagService, IDoctorService DoctorServices, INurseService NurseServices, ISickService SickServices,
            IIdentityRepository IdentityRepository, IDashboardService DashboardService, IAccountService AccountService, IHospitalService HospitalServices, StoreContext dbContext)
        {
            this.LikeService = LikeService;
            this.RatingService = RatingService;
            this.CommentService = CommentService;
            this.PostService = PostService;
            this.ConnectionService = ConnectionService;
            this.FollowService = FollowService;
            this.DoctorServices = DoctorServices;
            this.NurseServices = NurseServices;
            this.SickServices = SickServices;
            this.HospitalServices = HospitalServices;
            this.IdentityRepository = IdentityRepository;
            this.TagService = TagService;
            this.UserTagService = UserTagService;
            this.AccountService = AccountService;
            this.DashboardService = DashboardService;
            _dbContext = dbContext;
        }
    }
    public interface IUnitOfWork
    {
        public ILikeService LikeService { get; }
        public IRatingService RatingService { get; }
        public IConnectionService ConnectionService { get; }
        public ICommentService CommentService { get; }
        public IPostService PostService { get; }
        public IDoctorService DoctorServices { get; }
        public INurseService NurseServices { get; }
        public ISickService SickServices { get; }
        public IHospitalService HospitalServices { get; }
        public ITagService TagService { get; }
        public IUserTagService UserTagService { get; }
        public IFollowService FollowService { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IAccountService AccountService { get; }
        public IDashboardService DashboardService { get; }
    }
}