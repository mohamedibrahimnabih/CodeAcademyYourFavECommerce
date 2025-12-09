using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class TicketWithRelatedVM
    {
        public List<Message> Messages { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
