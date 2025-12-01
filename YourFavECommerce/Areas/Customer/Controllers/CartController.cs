using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CartController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();

            var carts = _context.Carts.Include(e=>e.Product).Where(e => e.ApplicationUserId == user.Id);

            return View(carts.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();

            var product = _context.Products.FirstOrDefault(e => e.Id == productId);

            if (product == null) return NotFound();

            var cartInDb = _context.Carts.FirstOrDefault(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if(cartInDb is not null)
            {
                cartInDb.Count += count;
            }
            else
            {
                _context.Carts.Add(new()
                {
                    ApplicationUserId = user.Id,
                    Count = count,
                    ProductId = productId,
                    Price = product.Price
                });
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult IncrementItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);
            if(cart is null) return NotFound();

            cart.Count += 1;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DecrementItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);
            if (cart is null) return NotFound();

            if(cart.Count > 1)
            {
                cart.Count -= 1;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteItem(int id)
        {
            var cart = _context.Carts.FirstOrDefault(e => e.Id == id);

            if (cart is null) return NotFound();

            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
