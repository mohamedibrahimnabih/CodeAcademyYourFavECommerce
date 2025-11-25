using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
        private readonly IEmailSender _emailSender;
        private UserManager<ApplicationUser> _userManager;// = new();

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var link = Url.Action(nameof(Confirm), "Account", new { area = "Identity", applicationUser.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(applicationUser.Email, "Pleas Confirm Your Account In Ecommerce Code Academy App",
                $"<h1>Please Confirm You Account By clicking <a href='{link}'>Here</a></h1>");


            // Print Success msg
            TempData["success-notification"] = "Add Account Successfully";

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Confirm(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var reuslt = await _userManager.ConfirmEmailAsync(user, token);

            if (!reuslt.Succeeded)
                TempData["error-notification"] = "Invalid Verficition";
            else
                TempData["success-notification"] = "Confirm You Account Successfully";

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

            // Check Email Or UserName
            var user = await _userManager.FindByEmailAsync(loginVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(loginVM.EmailOrUserName);

            // Return Invalid Email Or UserName
            if (user is null)
            {
                ModelState.AddModelError("EmailOrUserName", "Invalid Email Or User Name");
                return View(loginVM);
            }

            #region Solve 1
            //// Check Password
            //var result = await _userManager.CheckPasswordAsync(user, loginVM.Password);

            //// Return Invalid Password msg
            //if (!result)
            //{
            //    ModelState.AddModelError("Password", "Invalid Password");
            //    return View(loginVM);
            //}

            //// Sign In
            //await _signInManager.SignInAsync(user, loginVM.RememberMe);
            #endregion

            #region Solve 2

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("EmailOrUserName", "Too many Attempts, Please try again later");
                    return View(loginVM);
                }
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View(loginVM);
            }

                #endregion

            // Return success msg
            TempData["success-notification"] = "Welcome Back";

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
