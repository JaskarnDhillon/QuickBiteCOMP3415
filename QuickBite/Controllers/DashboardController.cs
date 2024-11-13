using Microsoft.AspNetCore.Mvc;
using QuickBite.Models;
using System.Diagnostics;

namespace QuickBite.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public DashboardController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Route("/dashboard")]
        public IActionResult Index()
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