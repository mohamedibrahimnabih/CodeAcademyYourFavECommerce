using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context = new();

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string productName, long? minPrice, long? maxPrice, int? categoryId, int? brandId, bool isHot, int page = 1)
        {
            if (_roleManager.Roles.IsNullOrEmpty())
            {
                await _roleManager.CreateAsync(new(SD.SUPER_ADMIN_ROLE));
                await _roleManager.CreateAsync(new(SD.ADMIN_ROLE));
                await _roleManager.CreateAsync(new(SD.EMPLOYEE));
                await _roleManager.CreateAsync(new(SD.CUSTOMER));

                await _userManager.CreateAsync(new()
                {
                    UserName = "SuperAdmin",
                    Name = "SuperAdmin",
                    Email = "SuperAdmin@codeacademy.com",
                    EmailConfirmed = true,
                }, "Admin123$");

                var user = await _userManager.FindByNameAsync("SuperAdmin");

                await _userManager.AddToRoleAsync(user, SD.SUPER_ADMIN_ROLE);
            }


            var products = _context.Products.AsNoTracking().Include(e => e.Category).AsQueryable();

            if (productName is not null)
                products = products.Where(e => e.Name.Contains(productName));

            if(minPrice is not null)
                products = products.Where(e=>e.Price > minPrice);

            if(maxPrice is not null)
                products = products.Where(e => e.Price < maxPrice);

            if (categoryId is not null)
                products = products.Where(e => e.CategoryId == categoryId);

            if(brandId is not null)
                products = products.Where(e => e.BrandId == brandId);

            if(isHot)
                products = products.Where(e => e.Discount > 20);

            // Pagination
            var totalPages = Math.Ceiling(products.Count() / 8.0); // 3.25 => 4
            products = products.Skip((page - 1) * 8).Take(8); // 1

            var categories = _context.Categories.Include(e=>e.Products).AsNoTracking().AsQueryable();
            var brands = _context.Brands.AsNoTracking().AsQueryable();

            ProductWithRelatedVM productWithRelatedVM = new()
            {
                Products = products.ToList(),
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                TotalPages = totalPages,
                CurrentPage = page
            };

            return View(productWithRelatedVM);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(e => e.Id == id);
            
            if (product is null)
                return NotFound();

            product.Traffic += 1;
            _context.SaveChanges();

            var productInSameCategory = _context.Products.Include(e=>e.Category).Where(e => e.CategoryId == product.CategoryId && e.Id != product.Id).Skip(0).Take(4);

            var productInSameBrand = _context.Products.Include(e=>e.Category).Where(e => e.BrandId == product.BrandId && e.Id != product.Id).Skip(0).Take(4);

            var productsInSameName = _context.Products.Include(e => e.Category).Where(e => e.Name.ToLower().Contains(product.Name) && e.Id != product.Id).Skip(0).Take(4);

            var minPrice = product.Price - ( product.Price * 0.1m );
            var maxPrice = product.Price + (product.Price * 0.1m);
            var productInSamePrice = _context.Products.Include(e => e.Category).Where(e => e.Price > minPrice && e.Price < maxPrice && e.Id != product.Id);

            var topProudcts = _context.Products.Include(e=>e.Category).Where(e=>e.Id != product.Id).OrderByDescending(e=>e.Traffic).Skip(0).Take(4);

            ProductDetailsVM productDetailsVM = new()
            {
                Product = product,
                ProductInSameCategory = productInSameCategory.ToList(),
                ProductInSameName = productsInSameName.ToList(),
                ProductInSamePrice = productInSamePrice.ToList(),
                TopProducts = topProudcts.ToList(),
                ProductInSameBrand = productInSameBrand.ToList(),
            };

            return View(productDetailsVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
