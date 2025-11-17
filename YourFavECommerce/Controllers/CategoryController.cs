using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModel;

namespace YourFavECommerce.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(string name, int page = 1) // Mobiles
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();

            if (name is not null)
                categories = categories.Where(e => e.Name.ToLower().Contains(name.ToLower().Trim()));

            //if (status)
            //    categories = categories.Where(e => e.Status);
            //else
            //    categories = categories.Where(e => !e.Status);

            //categories = status ? categories.Where(e => e.Status) : categories.Where(e => !e.Status);

            double totalPages = Math.Ceiling(categories.Count() / 5.0);
            int currentPage = page;

            CategoryWithRelatedVM categoryWithRelatedVM = new()
            {
                Categories = categories.Skip((page - 1) * 5).Take(5).ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return View(categoryWithRelatedVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            if (category is null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
