using System.Reflection;
using API.Extensions;
using API.Hubs;
using DAL.DataContext;
using DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Seed;


var builder = WebApplication.CreateBuilder(args);
IConfiguration config = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddDBContextServices(config);
builder.Services.AddAutoMapperServices();
builder.Services.AddInjectionServices();
builder.Services.AddIdentityServices(config);
builder.Services.AddSwaggerServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddCors();
builder.Services.AddSignalR();
var app = builder.Build();

// for migrate data to database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        await context.Database.MigrateAsync();
        await RoleSeed.SeedRoleAsync(roleManager);
        await UserSeed.SeedAdminAsync(userManager);
        await UserSeed.SeedSickAsync(userManager);
        await CitySeed.SeedCitiesAsync(context);
        await DoctorSeed.SeedDoctorsAsync(userManager, context);
        await NuresSeed.SeedNursesAsync(userManager, context);
        await HospitalSeed.SeedHospitalsAsync(userManager, context);
        await HospitalSeed.SeedDepartmentsAsync(context);
        await HospitalSeed.SeedBedsAsync(context);
        await PostSeed.SeedPostsAsync(context);
        await RatingSeed.SeedRaingAsync(context);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "error happen when migration");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("../swagger/v1/swagger.json", "Syrian Medicine Project v1");
    });
}

app.UseCors(x => x.WithOrigins().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PublicHub>("/Publichub");
app.Run();
