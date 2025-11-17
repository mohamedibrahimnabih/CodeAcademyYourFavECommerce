using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;

namespace YourFavECommerce.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();

            return View(categories.ToList());
        }
    }
}
