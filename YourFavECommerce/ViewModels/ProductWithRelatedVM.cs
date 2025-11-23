using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class ProductWithRelatedVM
    {
        public List<Product> Products { get; set; } = default!;
        public List<Brand> Brands { get; set; } = default!;
        public List<Category> Categories { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
