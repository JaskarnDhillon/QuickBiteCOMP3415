using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    }
}
