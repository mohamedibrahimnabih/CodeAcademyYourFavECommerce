using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class OrderWithRelatedVM
    {
        public List<Order> Orders { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
