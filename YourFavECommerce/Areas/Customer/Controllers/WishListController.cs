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
    public class WishListController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public WishListController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();

            var Wishlists = _context.Wishlists.Include(e=>e.Product).Where(e => e.ApplicationUserId == user.Id);

            return View(Wishlists.ToList());
        }

        //[HttpPost]
        public async Task<IActionResult> AddToWishList(int productId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) return NotFound();

            var Wishlists = _context.Wishlists.Include(e => e.Product).Where(e => e.ApplicationUserId == user.Id);

            bool isFounded = false;

            foreach (var item in Wishlists)
            {
                if(item.ProductId == productId)
                {
                    isFounded = true;
                    break;
                }
            }

            if (!isFounded)
            {
                _context.Wishlists.Add(new()
                {
                    ProductId = productId,
                    ApplicationUserId = user.Id
                });

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteItem(int id)
        {
            var wishList = _context.Wishlists.FirstOrDefault(e => e.Id == id);

            if(wishList == null) return NotFound();

            _context.Wishlists.Remove(wishList);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
