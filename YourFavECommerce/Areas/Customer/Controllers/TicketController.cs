using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class TicketController: Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TicketController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(FilterVM filterVM)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();


            var tickets = _context.Messages.Where(e => e.SenderId == user.Id);

            if (filterVM.Name is not null)
                tickets = tickets.Where(e => e.Text.ToLower().Contains(filterVM.Name.ToLower().Trim()));

            double totalPages = Math.Ceiling(tickets.Count() / 5.0);
            int currentPage = filterVM.Page;

            return View(new TicketWithRelatedVM()
            {
                Messages = tickets.ToList(),
                CurrentPage = currentPage,
                TotalPages = totalPages,
            });
        }
    }
}
