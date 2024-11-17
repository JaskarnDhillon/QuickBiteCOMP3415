using Microsoft.AspNetCore.Mvc;
using QuickBite.Models;
using System.Diagnostics;
using QuickBite.Data;

namespace QuickBite.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }
        [Route("/dashboard")]
        public IActionResult Index()
        {
            
            return View();
            
        }
        [Route("/orders")]
        public IActionResult Orders()
        {
            var restaurants = _context.Restaurant.ToList();
            
            return View(restaurants);

        }
        [Route("/offers")]
        public IActionResult Offers()
        {

            return View();

        }
        [Route("/favourites")]
        public IActionResult Favourites()
        {

            return View();

        }
        [Route("/settings")]
        public IActionResult Settings()
        {

            return View();

        }

        /*[Route("/about")]
        public IActionResult About()
        {
            return View();
        }*/


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}