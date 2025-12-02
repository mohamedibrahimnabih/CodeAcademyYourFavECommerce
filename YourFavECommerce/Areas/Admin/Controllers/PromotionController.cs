using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(FilterVM filterVM)
        {
            var promotions = _context.Promotions.AsQueryable();

            if(filterVM.Name is not null)
                promotions = promotions.Where(e => e.Code.ToLower().Contains(filterVM.Name.ToLower().Trim()));


            double totalPages = Math.Ceiling(promotions.Count() / 5.0);
            int currentPage = filterVM.Page;

            PromotionWithRelatedVM promotionWithRelatedVM = new()
            {
                Promotions = promotions.Skip((filterVM.Page - 1) * 5).Take(5).ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return View(promotionWithRelatedVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var products = _context.Products.ToList();
            ViewBag.products = products;

            return View(new Promotion());
        }

        [HttpPost]
        public IActionResult Create(Promotion promotion)
        {
            var promotions = _context.Promotions.ToList();

            foreach(var item in promotions)
            {
                if(item.Code == promotion.Code)
                {
                    ModelState.AddModelError("Code", "Duplicate Code");
                    var products = _context.Products.ToList();
                    ViewBag.products = products;
                    return View(promotion);
                }
            }

            _context.Promotions.Add(promotion);
            _context.SaveChanges();

            // Success msg
            //Response.Cookies.Append("success-notifications", "Add Category Successfully");
            TempData["success-notifications"] = "Add Promotion Successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(int id)
        {
            var promotion = _context.Promotions.FirstOrDefault(e => e.Id == id);

            if (promotion is null) return NotFound();

            var products = _context.Products.ToList();
            ViewBag.products = products;

            return View(promotion);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(Promotion promotion)
        {
            var promotions = _context.Promotions.ToList();

            foreach (var item in promotions)
            {
                if (item.Code == promotion.Code)
                {
                    ModelState.AddModelError("Code", "Duplicate Code");
                    var products = _context.Products.ToList();
                    ViewBag.products = products;
                    return View(promotion);
                }
            }

            _context.Promotions.Update(promotion);
            _context.SaveChanges();

            // Success msg
            //Response.Cookies.Append("success-notifications", "Update Category Successfully");
            TempData["success-notifications"] = "Update Promotion Successfully";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Delete(int id)
        {
            var promotion = _context.Promotions.FirstOrDefault(e => e.Id == id);

            if (promotion is null)
                return NotFound();

            _context.Promotions.Remove(promotion);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
