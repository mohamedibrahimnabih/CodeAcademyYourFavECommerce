using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModel;

namespace YourFavECommerce.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(string name, decimal? minPrice, decimal? maxPrice, int? categoryId, int page = 1)
        {
            var products = _context.Products.AsNoTracking().Include(e => e.Category).Include(e => e.Brand).AsQueryable();

            if(name is not null) 
                products = products.Where(e=>e.Name.ToLower().Contains(name.ToLower().Trim()));

            if (minPrice is not null)
                products = products.Where(e => e.Price > minPrice);

            if (maxPrice is not null)
                products = products.Where(e => e.Price < minPrice);

            if (categoryId is not null)
                products = products.Where(e => e.CategoryId == categoryId);

            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            ProductWithRelatedVM productWithRelatedVM = new()
            {
                Products = products.Skip((page - 1) * 5).Take(5).ToList(),
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                CurrentPage = page,
                TotalPages = Math.Ceiling(products.Count() / 5.0)
            };

            return View(productWithRelatedVM);
        }
    }
}
