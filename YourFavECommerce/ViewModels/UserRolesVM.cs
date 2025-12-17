using Microsoft.AspNetCore.Identity;
using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class UserRolesVM
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public List<string> UserRoles { get; set; }
        public List<IdentityRole> Roles { get; set; }
    }
}
