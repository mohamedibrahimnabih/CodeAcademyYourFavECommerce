using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(FilterVM filterVM)
        {
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            if (filterVM.Name is not null)
                brands = brands.Where(e => e.Name.ToLower().Contains(filterVM.Name.ToLower().Trim()));

            double totalPages = Math.Ceiling(brands.Count() / 5.0);
            int currentPage = filterVM.Page;

            BrandWithRelatedVM brandWithRelatedVM = new()
            {
                Brands = brands.Skip((filterVM.Page - 1) * 5).Take(5).ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return View(brandWithRelatedVM);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(CreateBrandWithLogoVM createBrandWithLogoVM)
        {
            Brand brand = new()
            {
                Name = createBrandWithLogoVM.Name,
                Status = createBrandWithLogoVM.Status
            };

            if (createBrandWithLogoVM.Logo is not null && createBrandWithLogoVM.Logo.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(createBrandWithLogoVM.Logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(createBrandWithLogoVM.Logo.FileName)}";

                brand.logo = fileName;

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    createBrandWithLogoVM.Logo.CopyTo(stream);
                }
            }

            _context.Brands.Add(brand);
            _context.SaveChanges();

            TempData["success-notifications"] = "Add Brand Successfully";

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
        public IActionResult Edit(UpdateBrandWithLogoVM updateBrandWithLogoVM)
        {
            Brand brand = new()
            {
                Id = updateBrandWithLogoVM.Id,
                Name = updateBrandWithLogoVM.Name,
                Status = updateBrandWithLogoVM.Status
            };

            var brandInDB = _context.Brands.AsNoTracking().FirstOrDefault(e => e.Id == updateBrandWithLogoVM.Id);

            if (brandInDB is null)
                return NotFound();

            if (updateBrandWithLogoVM.Logo is not null)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(updateBrandWithLogoVM.Logo.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(updateBrandWithLogoVM.Logo.FileName)}";

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
                    updateBrandWithLogoVM.Logo.CopyTo(stream);
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

            TempData["success-notifications"] = "Update Brand Successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(e => e.Id == id);

            if (brand is null)
                return NotFound();

            // Remove old img from wwwroot
            string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\brand_logos", brand.logo);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _context.Brands.Remove(brand);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
