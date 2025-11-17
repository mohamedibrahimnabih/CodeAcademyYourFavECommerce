using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModel;

namespace YourFavECommerce.Controllers
{
    public class BrandController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(string name, int page = 1)
        {
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            if (name is not null)
                brands = brands.Where(e => e.Name.ToLower().Contains(name.ToLower().Trim()));

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand)
        {
            _context.Brands.Add(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //}

        //[HttpPost]
        //public IActionResult Edit(Brand brand)
        //{
        //}

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null)
                return NotFound();

            _context.Brands.Remove(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
