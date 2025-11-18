using Microsoft.AspNetCore.Http;
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
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Brand brand, IFormFile logo)
        {
            if (logo is not null && logo.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(logo.FileName)}";

                brand.logo = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    logo.CopyTo(stream);
                }
            }

            _context.Brands.Add(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null)
                return NotFound();

            return View(brand);
        }

        [HttpPost]
        public IActionResult Edit(Brand brand, IFormFile? logo)
        {
            var brandInDB = _context.Brands.AsNoTracking().FirstOrDefault(e => e.Id == brand.Id);

            if (brandInDB is null)
                return NotFound();

            if (logo is not null)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(logo.FileName)}";

                // Remove old img from wwwroot
                string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", brandInDB.logo);

                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);

                brand.logo = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    logo.CopyTo(stream);
                }
            }
            else
                brand.logo = brandInDB.logo;

            // Solution 1
            brand.UpdatedAT = DateTime.Now;
            _context.Brands.Update(brand);

            // Solution 2
            //brand.UpdatedAT = DateTime.Now;
            //brand.Name = brand.Name;
            //brand.Status = brand.Status;
            //brand.CreateAT = brand.CreateAT;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

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
