using YourFavECommerce.Models;

namespace YourFavECommerce.ViewModels
{
    public class CategoryWithRelatedVM
    {
        public List<Category> Categories { get; set; } = default!;
        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
