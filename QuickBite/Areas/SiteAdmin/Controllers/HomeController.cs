using Microsoft.AspNetCore.Mvc;

namespace QuickBite.Areas.SiteAdmin.Controllers
{
    public class HomeController : Controller
    {
        [Area("SiteAdmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
