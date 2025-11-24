using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class ProductDetailsVM
    {
        public Product Product { get; set; } = default!;
        public List<Product>? ProductInSameCategory { get; set; }
        public List<Product>? ProductInSameBrand { get; set; }
        public List<Product>? ProductInSameName { get; set; }
        public List<Product>? ProductInSamePrice { get; set; }
        public List<Product>? TopProducts { get; set; }
    }
}
