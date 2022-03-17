using DAL.DataContext;
using DAL.Entities.Identity.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Seed.GenerateData;

namespace Services.Seed
{
    public class RatingSeed
    {
        public static async Task SeedRaingAsync(StoreContext dbContext)
        {
            if (!await dbContext.Rate.AnyAsync())
            {
                var sicks = await dbContext.Users.Where(e => e.UserType == UserType.Sick).ToListAsync();
                var doctors = await dbContext.Users.Where(e => e.UserType == UserType.Doctor).ToListAsync();
                var nurses = await dbContext.Users.Where(e => e.UserType == UserType.Nurse).ToListAsync();
                var hospitals = await dbContext.Users.Where(e => e.UserType == UserType.Hospital).ToListAsync();

                var data = GenerateRatingForUsers.AddRating(sicks, doctors, nurses, hospitals);
                await dbContext.Rate.AddRangeAsync(data);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}