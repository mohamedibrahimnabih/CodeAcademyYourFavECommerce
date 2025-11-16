using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModel
{
    public class ProductWithRelatedVM
    {
        public List<Product> Products { get; set; } = default!;
        public List<Brand> Brands { get; set; } = default!;
        public List<Category> Categories { get; set; } = default!;
    }
}
