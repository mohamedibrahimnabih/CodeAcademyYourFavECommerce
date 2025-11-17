using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.ViewModel;

namespace YourFavECommerce.Controllers
{
    public class BrandController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(/* YOUR CODE HERE */ int page = 1)
        {
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            /* YOUR CODE HERE */

            double totalPages = Math.Ceiling(brands.Count() / 5.0);
            int currentPage = page;

            BrandWithRelatedVM brandWithRelatedVM = new()
            {
                Brands = brands.Skip((page - 1) * 5).Take(5).ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return View(brandWithRelatedVM);
        }
    }
}
