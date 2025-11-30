using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using YourFavECommerce.Models;

namespace YourFavECommerce.Utilites
{
    public class DBInitializer : IDBInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(new(SD.SUPER_ADMIN_ROLE));
                await _roleManager.CreateAsync(new(SD.ADMIN_ROLE));
                await _roleManager.CreateAsync(new(SD.EMPLOYEE));
                await _roleManager.CreateAsync(new(SD.CUSTOMER));

                await _userManager.CreateAsync(new()
                {
                    UserName = "SuperAdmin",
                    Name = "SuperAdmin",
                    Email = "SuperAdmin@codeacademy.com",
                    EmailConfirmed = true,
                }, "Admin123#");

                var user = await _userManager.FindByNameAsync("SuperAdmin");

                await _userManager.AddToRoleAsync(user, SD.SUPER_ADMIN_ROLE);
            }
        }
    }
}
