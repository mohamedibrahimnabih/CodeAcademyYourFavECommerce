using Microsoft.EntityFrameworkCore;

namespace YourFavECommerce.Models
{
    [PrimaryKey(nameof(Color), nameof(ProductId))]
    public class ProductColor
    {
        public string Color { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
    }
}
