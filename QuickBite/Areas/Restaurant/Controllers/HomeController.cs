using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Areas.Restaurant.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        [Area("Restaurant")]
        public IActionResult Index()
        {
            if (_userManager.GetUserAsync(User).Result.RestaurantId == null)
            {
                return Unauthorized();
            }

            return View();
        }
    }
}
