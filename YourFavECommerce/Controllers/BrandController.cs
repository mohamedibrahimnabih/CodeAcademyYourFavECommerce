using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;

namespace YourFavECommerce.Controllers
{
    public class BrandController : Controller
    {
        private ApplicationDbContext _dbContext = new();

        public IActionResult Index()
        {
            var brands = _dbContext.Brands.AsNoTracking().AsQueryable();

            return View(brands.ToList());
        }
    }
}
