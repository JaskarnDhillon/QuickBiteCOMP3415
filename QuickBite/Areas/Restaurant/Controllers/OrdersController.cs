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
    }
}
