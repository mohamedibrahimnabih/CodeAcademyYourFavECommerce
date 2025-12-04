using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourFavECommerce.Data;
using YourFavECommerce.Models;
using YourFavECommerce.Utilites;
using YourFavECommerce.ViewModels;

namespace YourFavECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE}")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(FilterVM filterVM)
        {
            var orders = _context.Orders.Include(e=>e.ApplicationUser).AsQueryable();

            if (filterVM.Name is not null)
                orders = orders.Where(e => e.ApplicationUser.Name.ToLower().Contains(filterVM.Name.ToLower().Trim()));


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

        public IActionResult Details(int id)
        {
            var order = _context.Orders.Include(e=>e.ApplicationUser).FirstOrDefault(e => e.Id == id);

            if (order is null) return NotFound();

            var orderItems = _context.OrderItems.Include(e=>e.Product).Where(e => e.OrderId == id);

            return View(new OrderWithItemsVM()
            {
                Order = order,
                OrderItems = orderItems.ToList()
            });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var order = _context.Orders.Include(e => e.ApplicationUser).FirstOrDefault(e => e.Id == id);

            if (order is null) return NotFound();

            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(int id, OrderStatus orderStatus, string trackingNumber, string carrierName)
        {
            if (!ModelState.IsValid)
                return View();

            var orderInDb = _context.Orders.Include(e => e.ApplicationUser).FirstOrDefault(e => e.Id == id);

            if (orderInDb is null) return NotFound();

            orderInDb.OrderStatus = orderStatus;
            orderInDb.TrackingNumber = trackingNumber;
            orderInDb.CarrierName = carrierName;
            orderInDb.ShippedDate = DateTime.UtcNow;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
