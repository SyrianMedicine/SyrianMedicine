using DAL.DataContext;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Seed.GenerateData;

namespace Services.Seed
{
    public class NuresSeed
    {
        public static async Task SeedNursesAsync(UserManager<User> userManager, StoreContext dbContext)
        {
            if (!await dbContext.Nurses.AnyAsync())
            {
                var data = GenerateNurses.AddNurses();
                foreach (var user in data.Item2)
                {
                    await userManager.CreateAsync(user, "1234@User");
                    await userManager.AddToRoleAsync(user, Roles.Nurse.ToString());

                }
                await dbContext.Nurses.AddRangeAsync(data.Item1);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}