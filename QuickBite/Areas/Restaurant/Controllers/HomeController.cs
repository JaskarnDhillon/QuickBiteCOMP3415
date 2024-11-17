using Microsoft.AspNetCore.Mvc;

namespace QuickBite.Areas.Restaurant.Controllers
{
    public class HomeController : Controller
    {
        [Area("Restaurant")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
