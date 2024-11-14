using Microsoft.AspNetCore.Mvc;
using QuickBite.Models;
using System.Diagnostics;

namespace QuickBite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            /*ExampleModel exampleModel1 = new ExampleModel();
            ExampleModel exampleModel2 = new ExampleModel();
            exampleModel1.title = "Example Title 1";
            exampleModel1.desc = "Example Desc 1";
            
            exampleModel2.title = "Example Title 2";
            exampleModel2.desc = "Example Desc 2";
            
            IEnumerable<ExampleModel> exampleModels = new List<ExampleModel>{exampleModel1, exampleModel2 };*/
            /*return View(exampleModels)*/
            return View();
            
        }

        public class ExampleModel()
        {
            public string title { get; set; }
            public string desc { get; set; }
        }
        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }
        

        [Route("/privacy-policy")]
        public IActionResult Privacy()
        {
            return View();
        }
        
        [Route("/terms-of-use")]
        public IActionResult TermsOfUse()
        {
            return View();
        }
        
        public IActionResult RedirectToDashboard()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
