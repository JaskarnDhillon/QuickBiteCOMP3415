using Microsoft.AspNetCore.Mvc;
using QuickBite.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;

namespace QuickBite.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; 

        public DashboardController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [Route("/dashboard")]
        public async Task<IActionResult> Index(int? productId)
        {
            if (productId == null)
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Restaurant)
                    .ToListAsync();

                return View(products);
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Restaurant)
                .FirstOrDefaultAsync(p => p.ProductId.Equals(productId));

            if (product == null)
            {
                return NotFound();
            }

            return View(new List<Product> { product });
        }
        [Route("/orders")]
        public IActionResult Orders()
        {

            return View();

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