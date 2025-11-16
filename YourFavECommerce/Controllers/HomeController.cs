using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModel;

namespace YourFavECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ApplicationDbContext _context = new();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string productName, decimal? minPrice, decimal? maxPrice, int? categoryId, int? brandId)
        {
            var products = _context.Products.AsNoTracking().Include(e => e.Category).AsQueryable();

            if (productName is not null)
                products = products.Where(e => e.Name.Contains(productName));

            if(minPrice is not null)
                products = products.Where(e=>e.Price > minPrice);

            if(maxPrice is not null)
                products = products.Where(e => e.Price < maxPrice);

            if (categoryId is not null)
                products = products.Where(e => e.CategoryId == categoryId);

            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            ProductWithRelatedVM productWithRelatedVM = new()
            {
                Products = products.ToList(),
                Categories = categories.ToList(),
                Brands = brands.ToList(),
            };

            return View(productWithRelatedVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
