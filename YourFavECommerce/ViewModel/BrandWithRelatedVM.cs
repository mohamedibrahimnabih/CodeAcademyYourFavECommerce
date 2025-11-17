using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModel
{
    public class BrandWithRelatedVM
    {
        public List<Brand> Brands { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
