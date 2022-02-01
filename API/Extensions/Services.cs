using DAL.DataContext;
using DAL.Entities;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services;
using Services.Profiles;

namespace API.Extensions
{
    public static class Services
    {
        public static IServiceCollection AddInjectionServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<DocumentsDoctor>), typeof(GenericRepository<DocumentsDoctor>));
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped(typeof(IGenericRepository<DocumentsNurse>), typeof(GenericRepository<DocumentsNurse>));
            services.AddScoped<INurseService, NurseService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DoctorProfile));
            services.AddAutoMapper(typeof(NurseProfile));
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
                // opt.OperationFilter<AppendAuthoriziton>();
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