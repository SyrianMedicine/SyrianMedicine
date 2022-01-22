using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Microsoft.AspNetCore.Identity;

namespace Services.Seed
{
    public class RoleSeed
    {
        public static async Task SeedRoleAsync(RoleManager<Role> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                List<Role> roles = new()
                {
                    new Role() { Name = Roles.Admin.ToString() },
                    new Role() { Name = Roles.Sick.ToString() },
                    new Role() { Name = Roles.Doctor.ToString() },
                    new Role() { Name = Roles.Hospital.ToString() },
                    new Role() { Name = Roles.Nurse.ToString() },
                    new Role() { Name = Roles.Secretary.ToString() },
                };
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }
}