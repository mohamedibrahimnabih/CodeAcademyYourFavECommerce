using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class OrderWithItemsVM
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
