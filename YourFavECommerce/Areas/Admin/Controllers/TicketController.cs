using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
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


            var tickets = _context.Messages.AsQueryable();

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

        public IActionResult MarkAsCompleted(int id)
        {
            var message = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (message is null) return NotFound();

            message.TicketStatus = TicketStatus.Completed;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Canceled(int id)
        {
            var message = _context.Messages.FirstOrDefault(e => e.Id == id);

            if (message is null) return NotFound();

            message.TicketStatus = TicketStatus.Canceled;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
