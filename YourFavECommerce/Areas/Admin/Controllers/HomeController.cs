using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourFavECommerce.Utilites;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
