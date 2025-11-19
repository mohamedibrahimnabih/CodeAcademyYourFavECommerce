using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModel;
using Mapster;

namespace YourFavECommerce.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(string name, decimal? minPrice, decimal? maxPrice, int? categoryId, int? brandId, int page = 1)
        {
            var products = _context.Products.AsNoTracking().Include(e => e.Category).Include(e => e.Brand).AsQueryable();

            if (name is not null)
                products = products.Where(e => e.Name.ToLower().Contains(name.ToLower().Trim()));

            if (minPrice is not null)
                products = products.Where(e => e.Price > minPrice);

            if (maxPrice is not null)
                products = products.Where(e => e.Price < minPrice);

            if (categoryId is not null)
                products = products.Where(e => e.CategoryId == categoryId);

            if (brandId is not null)
                products = products.Where(e => e.BrandId == brandId);

            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            ProductWithRelatedVM productWithRelatedVM = new()
            {
                Products = products.Skip((page - 1) * 5).Take(5).ToList(),
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                CurrentPage = page,
                TotalPages = Math.Ceiling(products.Count() / 5.0)
            };

            return View(productWithRelatedVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            return View(new ProductWithRelatedVM()
            {
                Categories = categories.ToList(),
                Brands = brands.ToList()
            });
        }

        [HttpPost]
        public IActionResult Create(CreateProductVM createProductVM)
        {
            //Product product = new()
            //{
            //    Name = createProductVM.Name,
            //    Description = createProductVM.Description,
            //    Price = createProductVM.Price,
            //    Status = createProductVM.Status,
            //    Discount = createProductVM.Discount,
            //    Quantity = createProductVM.Quantity,
            //    BrandId = createProductVM.BrandId,
            //    CategoryId = createProductVM.CategoryId,
            //};

            Product product = createProductVM.Adapt<Product>();

            if (createProductVM.MainImg is not null && createProductVM.MainImg.Length > 0)
            {
                #region Save Img in wwwroot

                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(createProductVM.MainImg.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(createProductVM.MainImg.FileName)}";

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);

                //if (!System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    createProductVM.MainImg.CopyTo(stream);
                }
                #endregion

                // Save Img in DB
                product.MainImg = fileName;
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            #region Save Sub Images in wwwroot & DB

            if(createProductVM.SubImages is not null && createProductVM.SubImages.Count > 0)
            {
                foreach (var item in createProductVM.SubImages)
                {
                    
                }
            }

            #endregion

            #region Save Colors in DB

            #endregion

            return RedirectToAction(nameof(Index));
        }
    }
}
