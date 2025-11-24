using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;// = new();
        private UserManager<ApplicationUser> _userManager;// = new();

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVM)
        {
            return View();
        }
    }
}
