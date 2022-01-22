using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly StoreContext _dbContext;
        public IdentityRepository(StoreContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<bool> CreateUserAsync(User inputUser, string password)
        {
            var dbRecord = await _userManager.FindByIdAsync(inputUser.Id);
            if (dbRecord != null)
                return false;
            await _userManager.CreateAsync(inputUser, password);
            return true;
        }

        public async Task<User> GetUserByIdAsync(string id) =>
            await _userManager.FindByIdAsync(id);

        public async Task<User> GetUserByNameAsync(string name) =>
            await _userManager.FindByNameAsync(name);

        public async Task<List<User>> GetUsersAsync() =>
            await _dbContext.Users.ToListAsync();

        public async Task<bool> RemoveUserByNameAsync(string name)
        {
            var dbRecord = await _userManager.FindByNameAsync(name);
            if (dbRecord == null)
                return false;
            await _userManager.DeleteAsync(dbRecord);
            return true;
        }

        public async Task<bool> RemoveUserByIdAsync(string id)
        {
            var dbRecord = await _userManager.FindByIdAsync(id);
            if (dbRecord == null)
                return false;
            await _userManager.DeleteAsync(dbRecord);
            return true;
        }

        public async Task<bool> UpdateUserAsync(User inputUser)
        {
            var dbRecord = await _userManager.FindByNameAsync(inputUser.UserName);
            if (dbRecord == null)
                return false;
            await _userManager.UpdateAsync(dbRecord);
            return true;
        }

        public async Task<Role> GetRoleByIdAsync(string id) =>
            await _roleManager.FindByIdAsync(id);

        public async Task<Role> GetRoleByNameAsync(string name) =>
            await _roleManager.FindByNameAsync(name);

        public async Task<List<Role>> GetRolesAsync() =>
            await _roleManager.Roles.ToListAsync();
        public IQueryable<Role> GetRolesQuery() =>
             _roleManager.Roles.AsQueryable<Role>();
        public async Task<User> GetUserByEmailAsync(string email) =>
            await _userManager.FindByEmailAsync(email);

        public async Task<bool> CreateRoleAsync(Role inputRole)
        {
            var dbRecord = await _roleManager.RoleExistsAsync(inputRole.Name);
            if (dbRecord == true)
                return false;
            await _roleManager.CreateAsync(inputRole);
            return true;
        }

        public async Task<bool> UpdateRoleAsync(Role inputRole)
        {
            var dbRecord = await _roleManager.FindByNameAsync(inputRole.Name);
            if (dbRecord == null)
                return false;
            await _roleManager.UpdateAsync(inputRole);
            return true;
        }

        public async Task<bool> DeleteRoleByIdAsync(string id)
        {
            var dbRecord = await _roleManager.FindByIdAsync(id);
            if (dbRecord == null)
                return false;
            await _roleManager.DeleteAsync(dbRecord);
            return true;
        }

        public async Task<bool> DeleteRoleByNameAsync(string name)
        {
            var dbRecord = await _roleManager.FindByNameAsync(name);
            if (dbRecord == null)
                return false;
            await _roleManager.DeleteAsync(dbRecord);
            return true;
        }

        public async Task<bool> LoginUser(User user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded;
        }
        public async Task<List<string>> GetRolesByUserIdAsync(string id)
        {
            List<string> result = new();
            var rolesName = await _dbContext.UserRoles.Where(x => x.UserId == id).ToListAsync();
            foreach (var item in rolesName)
            {
                var role = await _dbContext.Roles.Where(x => x.Id == item.RoleId).FirstOrDefaultAsync();
                result.Add(role.Name);
            }
            return result;
        }

        public async Task<bool> AddRoleToUserAsync(User user, string role)
        {
            await _userManager.AddToRoleAsync(user, role);
            return true;
        }

        public async Task<User> GetUserByUserClaim(ClaimsPrincipal userClaim)
        {
            var username = userClaim?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var user = await _userManager.FindByNameAsync(username);
            return user;
        }
        public async Task<bool> ChangePasssword(User user, string currentPassword, string newPassword)
        {
            await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return true;
        }
        public async Task<bool> CheckPassword(User user, string Password) =>
            await _userManager.CheckPasswordAsync(user, Password);

        public async Task<bool> DeleteRoleInUser(User user, string role)
        {
            await _userManager.RemoveFromRoleAsync(user, role);
            return true;
        }


    }
    public interface IIdentityRepository
    {
        public Task<bool> LoginUser(User user, string password);
        public Task<bool> CreateUserAsync(User inputUser, string password);
        public Task<bool> RemoveUserByNameAsync(string name);
        public Task<bool> RemoveUserByIdAsync(string id);
        public Task<bool> UpdateUserAsync(User inputUser);
        public Task<User> GetUserByIdAsync(string id);
        public Task<User> GetUserByNameAsync(string name);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<List<User>> GetUsersAsync();
        public Task<Role> GetRoleByIdAsync(string id);
        public Task<Role> GetRoleByNameAsync(string name);
        public Task<List<Role>> GetRolesAsync();
        public IQueryable<Role> GetRolesQuery();
        public Task<bool> CreateRoleAsync(Role inputRole);
        public Task<bool> UpdateRoleAsync(Role inputRole);
        public Task<bool> DeleteRoleByIdAsync(string id);
        public Task<bool> DeleteRoleByNameAsync(string name);
        public Task<List<string>> GetRolesByUserIdAsync(string id);
        public Task<bool> AddRoleToUserAsync(User user, string role);
        public Task<User> GetUserByUserClaim(ClaimsPrincipal userClaim);
        public Task<bool> ChangePasssword(User user, string currentPassword, string newPassword);
        public Task<bool> CheckPassword(User user, string Password);
        public Task<bool> DeleteRoleInUser(User user, string role);
    }
}