using System.Text.Json;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.Seed
{
    public class UserSeed
    {
        public static async Task SeedUserAsync(UserManager<User> userManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var userData = await File.ReadAllTextAsync(@"../Services/Services/Seed/Data/users.json");
                var users = JsonSerializer.Deserialize<List<User>>(userData);
                foreach (var user in users)
                {
                    User userForAdd = new()
                    {
                        City = user.City,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        HomeNumber = user.HomeNumber,
                        UserType = user.UserType,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        State = user.State,
                        Location = user.Location,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(userForAdd, "1234@Abc");
                    await userManager.AddToRoleAsync(userForAdd, Roles.Admin.ToString());
                }

            }
        }
    }
}