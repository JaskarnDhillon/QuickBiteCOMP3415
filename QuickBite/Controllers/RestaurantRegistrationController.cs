using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Controllers;

[Authorize]
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
    public async Task<IActionResult> Register([Bind("Name,Description,Address,DeliveryRadius,OpeningHour,CloseingHour")] Restaurant restaurant, IFormFile? photo)
    {
        try
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to identify the logged-in user.");
                return View("Index", restaurant);
            }

            // Assign restaurant owner
            restaurant.RestaurantOwner = user;
            restaurant.RestaurantOwenrId = user.Id;
            restaurant.isAccepted = false;
            restaurant.OpeningHour = restaurant.OpeningHour.ToUniversalTime();
            restaurant.CloseingHour = restaurant.CloseingHour.ToUniversalTime();

            // Handle photo upload
            if (photo != null && photo.Length > 0)
            {
                restaurant.Photo = UploadPhoto(photo);
            }

            // Save restaurant details
            _context.Restaurant.Add(restaurant);
            await _context.SaveChangesAsync();

            // Update user's restaurant association
            user.RestaurantId = restaurant.RestaurantId;
            await _userManager.UpdateAsync(user);

            // Store restaurant details in session
            HttpContext.Session.SetString("RestaurantId", restaurant.RestaurantId.ToString());
            HttpContext.Session.SetString("RestaurantName", restaurant.Name);

            return RedirectToAction("Index", "Home", new { area = "Restaurant" });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return View("Index", restaurant);
        }
    }

    private string UploadPhoto(IFormFile photo)
    {
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "restaurant-img-uploads");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{photo.FileName}";
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            photo.CopyTo(stream);
        }

        return uniqueFileName;
    }
}
