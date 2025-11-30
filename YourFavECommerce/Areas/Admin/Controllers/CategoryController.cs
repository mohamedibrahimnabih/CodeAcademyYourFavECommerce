using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class CategoryController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(FilterVM filterVM) // Mobiles
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();

            if (filterVM.Name is not null)
                categories = categories.Where(e => e.Name.ToLower().Contains(filterVM.Name.ToLower().Trim()));

            //if (status)
            //    categories = categories.Where(e => e.Status);
            //else
            //    categories = categories.Where(e => !e.Status);

            //categories = status ? categories.Where(e => e.Status) : categories.Where(e => !e.Status);

            double totalPages = Math.Ceiling(categories.Count() / 5.0);
            int currentPage = filterVM.Page;

            CategoryWithRelatedVM categoryWithRelatedVM = new()
            {
                Categories = categories.Skip((filterVM.Page - 1) * 5).Take(5).ToList(),
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

            // Success msg
            //Response.Cookies.Append("success-notifications", "Add Category Successfully");
            TempData["success-notifications"] = "Add Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(e => e.Id == id);

            if (category is null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(Category category)
        {
            category.UpdatedAT = DateTime.Now;

            _context.Categories.Update(category);
            _context.SaveChanges();

            // Success msg
            //Response.Cookies.Append("success-notifications", "Update Category Successfully");
            TempData["success-notifications"] = "Update Category Successfully";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
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
