using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user is null) return NotFound();

            //UserVM userVM = new()
            //{
            //    Address = user.Address,
            //    Email = user.Email,
            //    FullName = user.Name,
            //    PhoneNumber = user.PhoneNumber,
            //    UserName = user.UserName
            //};

            TypeAdapterConfig<ApplicationUser, UserVM>
                .NewConfig()
                .Map(d => d.FullName, s => s.Name);

            //TypeAdapterConfig typeAdapterConfig = new();
            //typeAdapterConfig.NewConfig<ApplicationUser, UserVM>()
            //    .Map("FullName", "Name");

            UserVM userVM = user.Adapt<UserVM>(/*typeAdapterConfig*/);

            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserVM userVM)
        {
            if (!ModelState.IsValid)
                return View(userVM);

            var user = await _userManager.FindByIdAsync(userVM.Id);

            if (user == null) return NotFound();

            user.Name = userVM.FullName;
            user.Email = userVM.Email;
            user.Address = userVM.Address;
            user.PhoneNumber = userVM.PhoneNumber;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string id, string currentPassword, string newPassword)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                TempData["error-notification"] = result.Errors;
                return View();
            }

            TempData["success-notification"] = "Change Password Successfully";
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateImage(string id, IFormFile Img)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null) return NotFound();

            //Upload Img in wwwroot
            if(Img is not null && Img.Length > 0)
            {
                // Save file name in DB
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Img.FileName);

                string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{fileNameWithoutExtension}_{Guid.NewGuid().ToString()}{Path.GetExtension(Img.FileName)}";

                // Save file in wwwroot
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\users", fileName);

                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Create(filePath);

                using (var stream = System.IO.File.Create(filePath))
                {
                    Img.CopyTo(stream);
                }

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\users", user.Img);
                if(System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                //Update Img Column in DB
                user.Img = fileName;
            }

            //Save Changes
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
