using API.Extensions;
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


var app = builder.Build();

// for migrate data to database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        var roleManager = services.GetRequiredService<RoleManager<Role>>(); await context.Database.MigrateAsync();
        await RoleSeed.SeedRoleAsync(roleManager);
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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
