using System.Collections.Generic;
using DAL.DataContext;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Seed.GenerateData;

namespace Services.Seed
{
    public class DoctorSeed
    {
        public static async Task SeedDoctorsAsync(UserManager<User> userManager, StoreContext dbContext)
        {
            if (!await dbContext.Doctors.AnyAsync())
            {
                var data = GenerateDoctors.AddDoctors();
                foreach (var user in data.Item2)
                {
                    await userManager.CreateAsync(user, "1234@User");
                    await userManager.AddToRoleAsync(user, Roles.Doctor.ToString());
                }
                await dbContext.Doctors.AddRangeAsync(data.Item1);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}