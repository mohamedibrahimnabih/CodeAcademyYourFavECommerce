using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class PromotionWithRelatedVM
    {
        public List<Promotion> Promotions { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
