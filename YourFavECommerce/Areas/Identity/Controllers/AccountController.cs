using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private ApplicationDbContext _context;// = new();
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;// = new();

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> Register()
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

            await _userManager.AddToRoleAsync(applicationUser, SD.CUSTOMER);


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
                TempData["error-notification"] = "Invalid Verification";
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

        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            if(!ModelState.IsValid)
                return View(resendEmailConfirmationVM);

            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.EmailOrUserName);

            if (user is null)
            {
                TempData["error-notification"] = "User Not Found";
                return View(resendEmailConfirmationVM);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(Confirm), "Account", new { area = "Identity", user.Id, token }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Resend - Pleas Confirm Your Account In Ecommerce Code Academy App",
                $"<h1>Please Confirm You Account By clicking <a href='{link}'>Here</a></h1>");

            TempData["success-notification"] = "Send Email Successfully";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(forgetPasswordVM);

            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.EmailOrUserName) ?? await _userManager.FindByNameAsync(forgetPasswordVM.EmailOrUserName);

            if (user is null)
            {
                TempData["error-notification"] = "User Not Found";
                return View(forgetPasswordVM);
            }

            //var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var link = Url.Action("NewPassword", "Account", new { area = "Identity", user.Id, token }, Request.Scheme);

            var otp = new Random().Next(1000, 9999);

            var count = _context.ApplicationUserOTPs.Count(e => e.ApplicationUserId == user.Id && e.CreatedAT >= DateTime.Now.AddHours(-24));



            if (count >= 4)
            {
                TempData["error-notification"] = "Too Many Atempts, Please Try Again Later";
                return View(forgetPasswordVM);
            }

            else
            {
                _context.ApplicationUserOTPs.Add(new ApplicationUserOTP()
                {
                    OTP = otp.ToString(),
                    ApplicationUserId = user.Id,
                });
                _context.SaveChanges();

                await _emailSender.SendEmailAsync(user.Email, "Please Password Reset Your Account In Ecommerce Code Academy App",
                    $"<h1>Password Reset By Using OTP {otp}</h1>");

                TempData["success-notification"] = "Send Email Successfully";

                return RedirectToAction(nameof(ValidateOTP), new { user.Id });
            }
        }

        [HttpGet]
        public IActionResult ValidateOTP(string id)
        {
            return View(new ValidateOTPVM()
            {
                Id = id,
            });
        }

        [HttpPost]
        public IActionResult ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
                return View(validateOTPVM);

            var otp = _context.ApplicationUserOTPs.OrderBy(e=>e.Id).LastOrDefault(e => e.ApplicationUserId == validateOTPVM.Id && !e.IsUsed && e.ValidTo > DateTime.UtcNow);

            if(otp.OTP == validateOTPVM.OTP)
            {
                otp.IsUsed = true;
                _context.SaveChanges();
                return RedirectToAction(nameof(NewPassword), new { validateOTPVM.Id });
            }

            TempData["error-notification"] = "Invalid OTP";
            return View();
        }

        [HttpGet]
        public IActionResult NewPassword(string id)
        {
            return View(new NewPasswordVM()
            {
                Id = id,
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (!ModelState.IsValid)
                return View(newPasswordVM);

           var user = await _userManager.FindByIdAsync(newPasswordVM.Id);

            if (user is null)
            {
                TempData["error-notification"] = "User Not Found";
                return View(newPasswordVM);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPasswordVM.Password);

            if(!result.Succeeded)
            {
                TempData["error-notification"] = result.Errors;
                return View(newPasswordVM);
            }

            TempData["success-notification"] = "Change Password Successfully";
            return RedirectToAction(nameof(Login));
        }
    }
}
