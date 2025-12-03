using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Threading.Tasks;
using YourFavECommerce.Data;
using YourFavECommerce.Models;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CheckoutController(IEmailSender emailSender, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Success(int id)
        {
            //////////////////////////
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
