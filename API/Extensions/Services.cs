using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services;
using Services.Profiles;
using Services.Services;

namespace API.Extensions
{
    public static class Services
    {

        public static IServiceCollection AddInjectionServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<DocumentsDoctor>), typeof(GenericRepository<DocumentsDoctor>));
            services.AddScoped(typeof(IGenericRepository<DocumentsNurse>), typeof(GenericRepository<DocumentsNurse>));
            services.AddScoped(typeof(IGenericRepository<DocumentsHospital>), typeof(GenericRepository<DocumentsHospital>));
            services.AddScoped(typeof(IGenericRepository<Department>), typeof(GenericRepository<Department>));
            services.AddScoped(typeof(IGenericRepository<Bed>), typeof(GenericRepository<Bed>));
            services.AddScoped(typeof(IGenericRepository<Tag>), typeof(GenericRepository<Tag>));
            services.AddScoped(typeof(IGenericRepository<UserTag>), typeof(GenericRepository<UserTag>));
            services.AddScoped(typeof(IGenericRepository<City>), typeof(GenericRepository<City>));
            services.AddScoped(typeof(IGenericRepository<Follow>), typeof(GenericRepository<Follow>));
            services.AddScoped(typeof(IGenericRepository<ReserveDoctor>), typeof(GenericRepository<ReserveDoctor>));
            services.AddScoped(typeof(IGenericRepository<ReserveNurse>), typeof(GenericRepository<ReserveNurse>));
            services.AddScoped(typeof(IGenericRepository<ReserveHospital>), typeof(GenericRepository<ReserveHospital>));
            services.AddScoped(typeof(IGenericRepository<HospitalDepartment>), typeof(GenericRepository<HospitalDepartment>));
            services.AddScoped(typeof(IGenericRepository<Doctor>), typeof(GenericRepository<Doctor>));
            services.AddScoped(typeof(IGenericRepository<Nurse>), typeof(GenericRepository<Nurse>));
            services.AddScoped(typeof(IGenericRepository<Post>), typeof(GenericRepository<Post>));
            services.AddScoped(typeof(IGenericRepository<PostTag>), typeof(GenericRepository<PostTag>));
            services.AddScoped(typeof(IGenericRepository<Bed>), typeof(GenericRepository<Bed>));
            services.AddScoped(typeof(IGenericRepository<Hospital>), typeof(GenericRepository<Hospital>));
            services.AddScoped(typeof(IGenericRepository<Department>), typeof(GenericRepository<Department>));
            services.AddScoped(typeof(IGenericRepository<UserConnection>), typeof(GenericRepository<UserConnection>));
            services.AddScoped(typeof(IGenericRepository<Comment>), typeof(GenericRepository<Comment>));
            services.AddScoped(typeof(IGenericRepository<AccountComment>), typeof(GenericRepository<AccountComment>));
            services.AddScoped(typeof(IGenericRepository<PostComment>), typeof(GenericRepository<PostComment>));
            services.AddScoped(typeof(IGenericRepository<SubComment>), typeof(GenericRepository<SubComment>));
            services.AddScoped(typeof(IGenericRepository<HospitalHistory>), typeof(GenericRepository<HospitalHistory>));
            services.AddScoped(typeof(IGenericRepository<Like>), typeof(GenericRepository<Like>));
            services.AddScoped(typeof(IGenericRepository<PostLike>), typeof(GenericRepository<PostLike>));
            services.AddScoped(typeof(IGenericRepository<CommentLike>), typeof(GenericRepository<CommentLike>));
            services.AddScoped(typeof(IGenericRepository<Rating>), typeof(GenericRepository<Rating>));
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<INurseService, NurseService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<INurseService, NurseService>();
            services.AddScoped<IHospitalService, HospitalService>();
            services.AddScoped<ISickService, SickService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IUserTagService, UserTagService>();
            services.AddScoped<IFollowService, FollowService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ILikeService, LikeService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<BackgroundTask>();
            return services;
        }
        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(CityProfile));
            services.AddAutoMapper(typeof(DoctorProfile));
            services.AddAutoMapper(typeof(AccountProfile));
            services.AddAutoMapper(typeof(NurseProfile));
            services.AddAutoMapper(typeof(SickProfile));
            services.AddAutoMapper(typeof(HospitalProfile));
            services.AddAutoMapper(typeof(TagProfile));
            services.AddAutoMapper(typeof(FollowProfile));
            services.AddAutoMapper(typeof(PostProfile));
            services.AddAutoMapper(typeof(CommentProfile));
            services.AddAutoMapper(typeof(LikeProfile));
            services.AddAutoMapper(typeof(UserCardProfile));
            return services;
        }
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Syrian Medicine Project", Version = "1.0" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Auth Bearer Scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                opt.AddSecurityDefinition(securitySchema.Reference.Id, securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {securitySchema , System.Array.Empty<string>()}
                };
                opt.AddSecurityRequirement(securityRequirement);

            });
            return services;
        }

        public static IServiceCollection AddDBContextServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<StoreContext>(x => x.UseSqlite(config.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}