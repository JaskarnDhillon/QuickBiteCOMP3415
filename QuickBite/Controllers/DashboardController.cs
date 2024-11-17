using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBite.Models;
using System.Diagnostics;
using static QuickBite.Controllers.HomeController;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using QuickBite.Data;

namespace QuickBite.Controllers
{

    public class NominatimResponse
    {
        public string lat { get; set; }  // Changed to string
        public string lon { get; set; }  // Changed to string
        public string display_name { get; set; }
    }
    public class DashboardController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private const double EARTH_RADIUS_KM = 6371;

        public DashboardController(
          ILogger<HomeController> logger,
          ApplicationDbContext context,
          IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "QuickBite-RestaurantApp");
        }

        public class DashboardIndexViewModel()
        {
           public List<Restaurant> Restaurants {  get; set; }
        }
        [Route("/dashboard")]
        public async Task<IActionResult> Index(string address)
        {

            var viewmodel = new DashboardIndexViewModel();
            // If no address provided, just show the empty dashboard
            if (string.IsNullOrEmpty(address))
            {
                return View(viewmodel);
            }

           
                // Geocoding part
                var encodedAddress = Uri.EscapeDataString(address);
                var geocodingUrl = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

                var response = await _httpClient.GetAsync(geocodingUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var geocodingResponse = JsonSerializer.Deserialize<List<NominatimResponse>>(content);

            if (geocodingResponse == null || !geocodingResponse.Any())
            {
                TempData["Error"] = "Could not find the specified address.";
                return View(viewmodel);
            }

            if (!double.TryParse(geocodingResponse[0].lat, out double customerLat) ||
                !double.TryParse(geocodingResponse[0].lon, out double customerLng))
            {
                //TempData["Error"] = "Invalid coordinates received from geocoding service.";
                return View(viewmodel);
            }

            await Task.Delay(1000);

              
                    // Get accepted restaurants with coordinates
                    var validRestaurants = _context.Restaurant
                        .Where(r => r.isAccepted && r.Latitude.HasValue && r.Longitude.HasValue)
                        .ToList();

                    // Calculate distances and filter
                    var restaurantsInRange = validRestaurants
                        .Select(r => new
                        {
                            Restaurant = r,
                            Distance = CalculateDistance(
                                customerLat,
                                customerLng,
                                r.Latitude.Value,
                                r.Longitude.Value
                            )
                        })
                        .Where(r => r.Distance <= r.Restaurant.DeliveryRadius)
                        .Select(r => r.Restaurant)
                        .ToList();

                    viewmodel.Restaurants = restaurantsInRange;
                    // Store results
                    //TempData["CustomerAddress"] = geocodingResponse[0].display_name;
                    //TempData["CustomerLat"] = customerLat;
                    //TempData["CustomerLng"] = customerLng;

                    // Serialize with error handling
                    //try
                    //{
                    //    var serializedRestaurants = JsonSerializer.Serialize(restaurantsInRange);
                    //    TempData["AvailableRestaurants"] = serializedRestaurants;
                    //}
                    //catch (Exception ex)
                    //{
                    //    _logger.LogError(ex, "Error serializing restaurants");
                    //    TempData["Error"] = "Error processing restaurant data.";
                    //    return View(new List<Restaurant>());
                    //}

                    return View(viewmodel);
                
        }

        private bool IsInRange(Restaurant restaurant, double customerLat, double customerLng)
        {
            if (!restaurant.Latitude.HasValue || !restaurant.Longitude.HasValue)
                return false;

            var distance = CalculateDistance(
                customerLat,
                customerLng,
                restaurant.Latitude.Value,
                restaurant.Longitude.Value
            );

            return distance <= restaurant.DeliveryRadius;
        }
        private double ToRad(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            lat1 = ToRad(lat1);
            lat2 = ToRad(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) *
                    Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EARTH_RADIUS_KM * c;
        }
        [Route("/orders")]
        public IActionResult Orders()
        {
            var restaurants = _context.Restaurant.ToList();
            
            return View(restaurants);

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
        
        public async Task<IActionResult> RestaurantMenu(Guid Id)
        {
            var products = await _context.Products.Where(p => p.RestaurantId == Id)
                .Include(p => p.Category)
                .ToListAsync();

            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}