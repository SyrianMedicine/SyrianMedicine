using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Seed.GenerateData;

namespace Services.Seed
{
    public class HospitalSeed
    {
        public static async Task SeedHospitalsAsync(UserManager<User> userManager, StoreContext dbContext)
        {
            var data = GenerateHospitals.AddHospitals();
            if (!await dbContext.Hospitals.AnyAsync())
            {
                foreach (var user in data.Item1)
                {
                    await userManager.CreateAsync(user, "1234@User");
                    await userManager.AddToRoleAsync(user, Roles.Hospital.ToString());
                }
                await dbContext.Hospitals.AddRangeAsync(data.Item2);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedDepartmentsAsync(StoreContext dbContext)
        {
            if (!await dbContext.Departments.AnyAsync())
            {
                var hospitals = await dbContext.Hospitals.ToListAsync();
                var data = GenerateHospitals.AddDepartments(hospitals);
                await dbContext.Departments.AddRangeAsync(data.Item1);
                await dbContext.HospitalsDepartments.AddRangeAsync(data.Item2);
                await dbContext.SaveChangesAsync();
            }
        }

        public static async Task SeedBedsAsync(StoreContext dbContext)
        {
            if (!await dbContext.Beds.AnyAsync())
            {
                var hospitals = await dbContext.Hospitals.ToListAsync();
                var departments = await dbContext.Departments.ToListAsync();
                var data = GenerateHospitals.AddBeds(departments, hospitals);
                await dbContext.Beds.AddRangeAsync(data);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}