using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;
using System.Net.Http;

namespace QuickBite.Areas.Restaurant.Controllers
{
    [Area("Restaurant")]
    public class RestaurantSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public RestaurantSettingsController(
           ApplicationDbContext context,
           UserManager<ApplicationUser> userManager,
           IHttpContextAccessor httpContextAccessor,
           IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "QuickBite-RestaurantApp");
        }


        public async Task<IActionResult> Index()
        {
            // get current user and send the restaurnt with the edit form

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var restaurant = _context.Restaurant.Where(r=>r.RestaurantOwenrId==user.Id).FirstOrDefault();
            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, QuickBite.Models.Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId)
            {
                return NotFound();
            }

            // Remove isAccepted from model state to prevent overwriting it
            ModelState.Remove("isAccepted");

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing restaurant entity from the database
                    var existingRestaurant = await _context.Restaurant.FindAsync(id);
                    if (existingRestaurant == null)
                    {
                        return NotFound();
                    }

                    // Update fields except isAccepted
                    existingRestaurant.Name = restaurant.Name;
                    existingRestaurant.Description = restaurant.Description;
                    existingRestaurant.Photo = restaurant.Photo;
                    existingRestaurant.Address = restaurant.Address;
                    existingRestaurant.Latitude = restaurant.Latitude;
                    existingRestaurant.Longitude = restaurant.Longitude;
                    existingRestaurant.DeliveryRadius = restaurant.DeliveryRadius;
                    existingRestaurant.OpeningHour = restaurant.OpeningHour.ToUniversalTime();
                    existingRestaurant.CloseingHour = restaurant.CloseingHour.ToUniversalTime();

                    // Save changes to the database
                    _context.Update(existingRestaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(restaurant);
        }

        private bool RestaurantExists(Guid id)
        {
            return _context.Restaurant.Any(e => e.RestaurantId == id);
        }

    }
}
