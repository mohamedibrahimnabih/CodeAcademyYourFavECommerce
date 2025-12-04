using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(FilterVM filterVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if(user is null) return NotFound();

            var orders = _context.Orders.Where(e=>e.ApplicationUserId == user.Id);

            if (filterVM.OrderId is not null)
                orders = orders.Where(e => e.Id == filterVM.OrderId);

            double totalPages = Math.Ceiling(orders.Count() / 5.0);
            int currentPage = filterVM.Page;

            OrderWithRelatedVM orderWithRelatedVM = new()
            {
                Orders = orders.Skip((filterVM.Page - 1) * 5).Take(5).ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            return View(orderWithRelatedVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            return View();
        }
    }
}
