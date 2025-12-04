using Mapster;
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
    public class ProductController : Controller
    {
        private ApplicationDbContext _context = new();

        public IActionResult Index(string name, long? minPrice, long? maxPrice, int? categoryId, int? brandId, int page = 1)
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

            return View(new ProductVM()
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
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FileName);

                    string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(item.FileName)}";

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images", fileName);

                    //if (!System.IO.File.Exists(filePath))
                    //    System.IO.File.Create(filePath);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    _context.ProductSubImgs.Add(new()
                    {
                        SubImg = fileName,
                        ProductId = product.Id
                    });
                }
                _context.SaveChanges();
            }

            #endregion

            #region Save Colors in DB

            if(createProductVM.Colors is not null && createProductVM.Colors.Count > 0)
            {
                foreach (var item in createProductVM.Colors)
                {
                    _context.ProductColors.Add(new()
                    {
                        Color = item,
                        ProductId = product.Id
                    });
                }
                _context.SaveChanges();
            }

            #endregion

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null)
                return NotFound();

            var categories = _context.Categories.AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            var productSubImages = _context.ProductSubImgs.Where(e => e.ProductId == id);
            var productColors = _context.ProductColors.Where(e => e.ProductId == id);

            return View(new ProductVM()
            {
                Product = product,
                Brands = brands.ToList(),
                Categories = categories.ToList(),
                ProductSubImgs = productSubImages.ToList(),
                ProductColors = productColors.ToList()
            });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Edit(UpdateProductVM updateProductVM)
        {
            var productInDb = _context.Products.FirstOrDefault(e => e.Id == updateProductVM.Id);

            if(productInDb is null)
                return NotFound();

            // Update Main Img if exist 
            if(updateProductVM.MainImg is not null)
            {
                if(updateProductVM.MainImg.Length > 0)
                {
                    #region Save Img in wwwroot

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(updateProductVM.MainImg.FileName);

                    string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(updateProductVM.MainImg.FileName)}";

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", fileName);

                    //if (!System.IO.File.Exists(filePath))
                    //    System.IO.File.Create(filePath);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        updateProductVM.MainImg.CopyTo(stream);
                    }

                    // Delete Old Img From wwwroot
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", productInDb.MainImg);

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    #endregion

                    // Save Img in DB
                    productInDb.MainImg = fileName;
                }
            }

            // Update Sub Imgs if exist 
            if(updateProductVM.SubImages is not null)
            {
                if(updateProductVM.SubImages.Count > 0)
                {
                    // Delete old sub imgs from wwwroot, DB
                    var oldSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == updateProductVM.Id);

                    foreach (var item in oldSubImgs)
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images", item.SubImg);

                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }
                    _context.ProductSubImgs.RemoveRange(oldSubImgs);

                    // Add New sub imgs in wwwroot, DB
                    foreach (var item in updateProductVM.SubImages)
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item.FileName);

                        string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(item.FileName)}";

                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        _context.ProductSubImgs.Add(new()
                        {
                            SubImg = fileName,
                            ProductId = updateProductVM.Id
                        });
                    }
                }
            }

            // Update Colors if exist 
            if(updateProductVM.Colors is not null)
            {
                if(updateProductVM.Colors.Count > 0)
                {
                    var oldColors = _context.ProductColors.Where(e => e.ProductId == updateProductVM.Id);

                    _context.ProductColors.RemoveRange(oldColors);

                    foreach (var item in updateProductVM.Colors)
                    {
                        _context.ProductColors.Add(new()
                        {
                            Color = item,
                            ProductId = updateProductVM.Id
                        });
                    }
                }
            }

            // Update Product 
            productInDb.Name = updateProductVM.Name;
            productInDb.Description = updateProductVM.Description;
            productInDb.Status = updateProductVM.Status;
            productInDb.Price = updateProductVM.Price;
            productInDb.Discount = updateProductVM.Discount;
            productInDb.Quantity = updateProductVM.Quantity;
            productInDb.CategoryId = updateProductVM.CategoryId;
            productInDb.BrandId = updateProductVM.BrandId;

            // Save Changes
            _context.SaveChanges();

            // Return
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult DeleteSubImg(int id)
        {
            var productSubImg = _context.ProductSubImgs.FirstOrDefault(e => e.Id == id);

            if (productSubImg is null)
                return NotFound();

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images", productSubImg.SubImg);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            _context.ProductSubImgs.Remove(productSubImg);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = productSubImg.ProductId });
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult DeleteColor(int id)
        {
            var productColorInDB = _context.ProductColors.FirstOrDefault(e => e.Id == id);

            if (productColorInDB is null)
                return NotFound();

            _context.ProductColors.Remove(productColorInDB);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = productColorInDB.ProductId });
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);

            if (product is null)
                return NotFound();

            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images", product.MainImg);

            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);

            var oldProductSubImgs = _context.ProductSubImgs.Where(e => e.ProductId == id);

            foreach (var item in oldProductSubImgs)
            {
                var oldSubImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images", item.SubImg);

                if (System.IO.File.Exists(oldSubImgPath))
                    System.IO.File.Delete(oldSubImgPath);
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
