using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;

namespace YourFavECommerce.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var products = _context.Products.AsNoTracking().Include(e => e.Category).Include(e => e.Brand).AsQueryable();

            return View(products.ToList());
        }
    }
}
