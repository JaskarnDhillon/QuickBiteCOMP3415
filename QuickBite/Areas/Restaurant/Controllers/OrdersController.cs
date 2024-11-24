using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    public class OrdersController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public OrdersController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           IHttpContextAccessor httpContextAccessor,
           IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "QuickBite-RestaurantApp");
        }
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var restaurant = _context.Restaurant.Where(r => r.RestaurantOwenrId == user.Id).FirstOrDefault();
            
            // get all the orders within this restaurant
            var orders = _context.Orders.Include(o=>o.OrderDetails)
                .ThenInclude(od=>od.Product).Where(o=>o.RestaurantId==restaurant.RestaurantId).ToList();

            return View(orders);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Fetch the restaurant for this user
            var restaurant = _context.Restaurant.FirstOrDefault(r => r.RestaurantOwenrId == user.Id);

            if (restaurant == null)
            {
                return NotFound();
            }

            // Fetch the specific order with its details
            var order = _context.Orders
                .Where(o=>o.RestaurantId==restaurant.RestaurantId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.RestaurantId == restaurant.RestaurantId && o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus status)
        {
            // Find the order
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Update the status
            order.OrderStatus = status;
            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
