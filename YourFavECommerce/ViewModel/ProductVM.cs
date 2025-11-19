using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModel
{
    public class ProductVM
    {
        public Product Product { get; set; } = default!;
        public List<Brand> Brands { get; set; } = default!;
        public List<Category> Categories { get; set; } = default!;
        public List<ProductSubImg>? ProductSubImgs { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
