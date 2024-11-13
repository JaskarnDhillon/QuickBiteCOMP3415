using Microsoft.AspNetCore.Mvc;
using QuickBite.Data;

namespace QuickBite.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET
    public IActionResult Index()
    {
        var restaurants = _context.Restaurant.ToList();
        return View(restaurants);
    }
}