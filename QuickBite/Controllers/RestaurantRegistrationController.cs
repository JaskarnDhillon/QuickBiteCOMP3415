using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Controllers;

public class RestaurantRegistrationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public RestaurantRegistrationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Name, Description, Photo, Address, Latitude, Longitude, DeliveryRadius, OpeningHour, CloseingHour")] Restaurant restaurant)
    {
        ApplicationUser applicationUser = new();
        try
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user != null)
            {
                applicationUser = user;
            }
        }
        catch (Exception ex)
        {
            string message = ex.Message;
        }

        restaurant.RestaurantOwner = applicationUser;
        restaurant.RestaurantOwenrId = applicationUser.Id;
        restaurant.isAccepted = false;
        
        _context.Restaurant.Add(restaurant);
        await _context.SaveChangesAsync();
        
        
        return RedirectToAction(nameof(Index));
    }
}