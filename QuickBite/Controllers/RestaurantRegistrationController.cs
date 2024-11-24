using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuickBite.Data;
using QuickBite.Models;
using System.Text.Json; // Added for JSON serialization

namespace QuickBite.Controllers
{
    [Authorize]
    public class RestaurantRegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public RestaurantRegistrationController(
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

        // GET
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
        [Bind("Name,Description,DeliveryRadius,OpeningHour,CloseingHour")] Restaurant restaurant,
        IFormFile? photo,
        string StreetName,
        string StreetNumber,
        string PostalCode,
        string PostalTown)
        {
            try
            {
                var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Unable to identify the logged-in user.");
                    return View("Index", restaurant);
                }

                // Combine address components
                var fullAddress = $"{StreetNumber} {StreetName}, {PostalCode} {PostalTown}";

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

                // Geocode the address
                var geocodeResult = await GeocodeAddress(fullAddress);
                if (geocodeResult != null)
                {
                    restaurant.Latitude = geocodeResult.Value.Latitude;
                    restaurant.Longitude = geocodeResult.Value.Longitude;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to geocode the address. Please ensure the address is correct.");
                    return View("Index", restaurant);
                }

                // Save restaurant details
                _context.Restaurant.Add(restaurant);
                _context.SaveChanges();

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

        private async Task<(double Latitude, double Longitude)?> GeocodeAddress(string address)
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var geocodingUrl = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

            var response = await _httpClient.GetAsync(geocodingUrl);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var geocodingResponse = JsonSerializer.Deserialize<List<NominatimResponse>>(content, options);
                if (geocodingResponse != null && geocodingResponse.Any())
                {
                    if (double.TryParse(geocodingResponse[0].lat, out double lat) &&
                        double.TryParse(geocodingResponse[0].lon, out double lon))
                    {
                        return (lat, lon);
                    }
                }
            }
            return null;
        }

        public class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
        }
    }
}
