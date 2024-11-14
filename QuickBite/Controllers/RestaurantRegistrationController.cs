using Microsoft.AspNetCore.Mvc;
using QuickBite.Models;

namespace QuickBite.Controllers;

public class RestaurantRegistrationController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Restaurant restaurant)
    {
        return RedirectToAction(nameof(Index));
    }
}