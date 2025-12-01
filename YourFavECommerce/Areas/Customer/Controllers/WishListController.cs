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
            /////////////////////////////////

            return RedirectToAction(nameof(Index));
        }
    }
}
