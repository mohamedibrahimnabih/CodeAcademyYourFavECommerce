using Microsoft.AspNetCore.Mvc;

namespace YourFavECommerce.Areas.Customer.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Success()
        {
            // Send Mail

            // Update Order Status

            // Create Order Items

            // Delete Cart

            // Decrease Product Quantity


            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
    }
}
