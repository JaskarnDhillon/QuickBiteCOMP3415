using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Areas.DeliveryDriver.Controllers
{
    [Area("DeliveryDriver")]
    public class HomeController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {

            if (_userManager.GetUserAsync(User).Result.RestaurantDeliveryDriverId == null)
            {
                return Unauthorized();
            }
            return View();
        }

        public IActionResult AssignedOrders()
        {
            var user = _userManager.GetUserAsync(User).Result.RestaurantDeliveryDriverId;
            if (user== null)
            {
                return Unauthorized();
            }
            // get the orders from restaurants where user is driver at and send the ones that are ready

            var restaurnt = _context.Restaurant.Include(r=>r.Orders).Where(r => r.RestaurantId==user).FirstOrDefault();

            var orders = restaurnt.Orders.Where(o=>o.OrderStatus==OrderStatus.Ready || o.OrderStatus==OrderStatus.OutForDelivery || o.OrderStatus==OrderStatus.Delivered).ToList();
            return View(orders);
        }

        public IActionResult Details(Guid id)
        {
            var driverRestaurantId = _userManager.GetUserAsync(User).Result.RestaurantDeliveryDriverId;

            if (driverRestaurantId == null)
            {
                return Unauthorized();
            }

            var order = _context.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefault(o => o.OrderId == id && o.Restaurant.RestaurantId == driverRestaurantId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderStatus(Guid id, OrderStatus status)
        {
            var driverRestaurantId = _userManager.GetUserAsync(User).Result.RestaurantDeliveryDriverId;

            if (driverRestaurantId == null)
            {
                return Unauthorized();
            }

            var order = _context.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefault(o => o.OrderId == id && o.Restaurant.RestaurantId == driverRestaurantId);

            if (order == null)
            {
                return NotFound();
            }

            // Update the order status
            order.OrderStatus = status;
            _context.SaveChanges();

            return RedirectToAction("AssignedOrders");
        }

    }
}
