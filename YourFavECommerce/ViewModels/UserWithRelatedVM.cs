using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class UserWithRelatedVM
    {
        public List<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
