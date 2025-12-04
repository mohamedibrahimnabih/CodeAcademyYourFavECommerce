using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Issuing;
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
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var orderDetails = _context.Orders.Include(e=>e.ApplicationUser).FirstOrDefault(e => e.Id == id && e.ApplicationUserId == user.Id);

            var items = _context.OrderItems.Include(e=>e.Product).Include(e=>e.Order).Where(e => e.OrderId == id);

            return View(new OrderWithItemsVM()
            {
                Order = orderDetails,
                OrderItems = items.ToList()
            });
        }

        public async Task<IActionResult> Refund(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var orderDetails = _context.Orders.FirstOrDefault(e => e.Id == id && e.ApplicationUserId == user.Id);

            if (orderDetails is null) return NotFound();

            var options = new RefundCreateOptions
            {
                PaymentIntent = orderDetails.TransactionId,
                Amount = orderDetails.TotalPrice * 1000,
                Reason = RefundReasons.Unknown
            };

            var service = new RefundService();
            var session = service.Create(options);

            orderDetails.OrderStatus = OrderStatus.Canceled;
            orderDetails.TransactionStatus = TransactionStatus.Refunded;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> ReOrder(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var order = _context.Orders.FirstOrDefault(e => e.Id == id && e.ApplicationUserId == user.Id);

            if (order is null) return NotFound();

            var items = _context.OrderItems.Include(e => e.Product).Where(e => e.OrderId == id).ToList();

            var cartInDb = _context.Carts.Where(e=>e.ApplicationUserId == user.Id).ToArray();

            foreach (var item in items)
            {
                bool isFounded = false;

                foreach (var cart in cartInDb)
                {
                    if (item.ProductId == cart.ProductId)
                    {
                        cart.Count += item.Count;
                        isFounded = true;
                    }
                }

                if(!isFounded)
                {
                    _context.Carts.Add(new()
                    {
                        ApplicationUserId = user.Id,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        Price = item.Price,
                    });
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        public IActionResult RateProduct(int orderItemId)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(e => e.Id == orderItemId);
            if (orderItem == null) return NotFound();
            return View(orderItem);
        }
    }
}
