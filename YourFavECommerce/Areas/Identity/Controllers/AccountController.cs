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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private UserManager<ApplicationUser> _userManager;// = new();

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if(!ModelState.IsValid)
                return View(registerVM);

            ApplicationUser applicationUser = new()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Name
            };

            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);

            if (!result.Succeeded)
            {
                // Print Errors
                TempData["error-notification"] = result.Errors.Select(e => e.Code);

                return View(registerVM);
            }

            // Send Email Confirmation

            // Print Success msg
            TempData["success-notification"] = "Add Account Successfully";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            /** YOUR CODE HERE **/

            // Return success msg
            TempData["success-notification"] = "Welcome Back";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
