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

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
            ///////////////////////////////

            return RedirectToAction(nameof(Index));
        }
    }
}
