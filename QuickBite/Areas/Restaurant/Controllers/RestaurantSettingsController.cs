using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
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

        public async Task<IActionResult> DeliveryDriver()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Fetch the restaurant for the logged-in user
            var restaurant = await _context.Restaurant
                .Where(r => r.RestaurantOwenrId == user.Id)
                .FirstOrDefaultAsync();
            // fetch delivery driver id
            var deliveryDriverId = restaurant.DeliveryDriverId;

            // find the user that has that dilevery driver id
            if (deliveryDriverId != null)
            {
                var driverEmail = _context.ApplicationUsers.Where(u => u.RestaurantDeliveryDriverId == deliveryDriverId).FirstOrDefault();
                if (driverEmail != null)
                {
                    ViewBag.DriverEmail = driverEmail;
                }
            }
            if (restaurant != null)
            {
                ViewBag.RestaurantId = restaurant.RestaurantId; // Pass the RestaurantId to the view
            }


            return View(restaurant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeliveryDriver(string email, Guid RestaurantId)
        {

            var restaurant = _context.Restaurant.Find(RestaurantId);

            if (string.IsNullOrWhiteSpace(email) || RestaurantId == Guid.Empty)
            {
                ModelState.AddModelError("", "Invalid input data.");
                return RedirectToAction(nameof(DeliveryDriver));
            }

            // Find the user by email
            var deliveryDriver = await _userManager.FindByEmailAsync(email);
            if (deliveryDriver == null)
            {
                ModelState.AddModelError("", "No user found with the provided email.");
                return RedirectToAction(nameof(DeliveryDriver));
            }

            // Assign the delivery driver to the restaurant
            deliveryDriver.RestaurantDeliveryDriverId = RestaurantId;
            restaurant.DeliveryDriverId = deliveryDriver.RestaurantDeliveryDriverId;
            

            var result = await _userManager.UpdateAsync(deliveryDriver);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(DeliveryDriver));
            }

            // Handle errors if the update fails
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction(nameof(DeliveryDriver));
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
