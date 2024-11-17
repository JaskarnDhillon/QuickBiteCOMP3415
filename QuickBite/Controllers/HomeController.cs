using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuickBite.Data;
using QuickBite.Models;
using System.Diagnostics;
using System.Text.Json;

namespace QuickBite.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private const double EARTH_RADIUS_KM = 6371;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "QuickBite-RestaurantApp");
        }

        public class NominatimResponse
        {
            public string lat { get; set; }  // Changed to string
            public string lon { get; set; }  // Changed to string
            public string display_name { get; set; }
        }

        //[HttpPost]
        //public async Task<IActionResult> RedirectToDashboard(string address)
        //{
        //    try
        //    {
        //        var encodedAddress = Uri.EscapeDataString(address);
        //        var geocodingUrl = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

        //        var response = await _httpClient.GetAsync(geocodingUrl);
        //        response.EnsureSuccessStatusCode();

        //        var content = await response.Content.ReadAsStringAsync();
        //        var geocodingResponse = JsonSerializer.Deserialize<List<NominatimResponse>>(content);

        //        if (geocodingResponse == null || !geocodingResponse.Any())
        //        {
        //            TempData["Error"] = "Could not find the specified address.";
        //            return RedirectToAction("Index");
        //        }

        //        // Parse strings to doubles
        //        if (!double.TryParse(geocodingResponse[0].lat, out double customerLat) ||
        //            !double.TryParse(geocodingResponse[0].lon, out double customerLng))
        //        {
        //            TempData["Error"] = "Invalid coordinates received from geocoding service.";
        //            return RedirectToAction("Index");
        //        }

        //        await Task.Delay(1000); // Respect rate limit

        //        Console.Write(customerLat);
        //        Console.Write(customerLng);


        //        var restaurants =  _context.Restaurant
        //            .Where(r => r.isAccepted &&
        //                       r.Latitude.HasValue &&
        //                       r.Longitude.HasValue)
        //            .Select(r => new
        //            {
        //                Restaurant = r,
        //                Distance = CalculateDistance(
        //                    customerLat,
        //                    customerLng,
        //                    r.Latitude.Value,
        //                    r.Longitude.Value
        //                )
        //            })


        //            .Where(r => r.Distance <= r.Restaurant.DeliveryRadius)
        //            .Select(r => r.Restaurant)
        //             .ToList();


        //        if (restaurants.Any())
        //        {
        //            Console.Write("ya");
        //        }
        //        // Store results
        //        TempData["CustomerAddress"] = geocodingResponse[0].display_name;
        //        TempData["CustomerLat"] = customerLat;
        //        TempData["CustomerLng"] = customerLng;
        //        TempData["AvailableRestaurants"] = JsonSerializer.Serialize(restaurants);

        //        return RedirectToAction("Index", "Dashboard");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error finding restaurants for address: {Address}", address);
        //        TempData["Error"] = "An error occurred while processing your request.";
        //        return RedirectToAction("Index");
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> RedirectToDashboard(string address)
        {
            try
            {
                // Geocoding part (kept the same)
                var encodedAddress = Uri.EscapeDataString(address);
                var geocodingUrl = $"https://nominatim.openstreetmap.org/search?q={encodedAddress}&format=json&limit=1";

                var response = await _httpClient.GetAsync(geocodingUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var geocodingResponse = JsonSerializer.Deserialize<List<NominatimResponse>>(content);

                if (geocodingResponse == null || !geocodingResponse.Any())
                {
                    TempData["Error"] = "Could not find the specified address.";
                    return RedirectToAction("Index");
                }

                if (!double.TryParse(geocodingResponse[0].lat, out double customerLat) ||
                    !double.TryParse(geocodingResponse[0].lon, out double customerLng))
                {
                    TempData["Error"] = "Invalid coordinates received from geocoding service.";
                    return RedirectToAction("Index");
                }

                await Task.Delay(1000);

                // Debug log the coordinates
               // _logger.LogInformation("Customer coordinates: Lat {Lat}, Lng {Lng}", customerLat, customerLng);

                try
                {
                    // First, let's check if we have any restaurants at all
                    var totalRestaurants = _context.Restaurant.Count();
                    _logger.LogInformation("Total restaurants in database: {Count}", totalRestaurants);

                    // Get accepted restaurants with coordinates
                    var validRestaurants =  _context.Restaurant
                        .Where(r => r.isAccepted && r.Latitude.HasValue && r.Longitude.HasValue)
                        .ToList();

                    //_logger.LogInformation("Valid restaurants (accepted with coordinates): {Count}", validRestaurants.Count);

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

                   // _logger.LogInformation("Restaurants in delivery range: {Count}", restaurantsInRange.Count);

                    // Log some details about the first few restaurants for debugging
                    foreach (var restaurant in restaurantsInRange.Take(3))
                    {
                        _logger.LogInformation(
                            "Restaurant: {Name}, Coords: ({Lat}, {Lng}), Radius: {Radius}km",
                            restaurant.Name,
                            restaurant.Latitude,
                            restaurant.Longitude,
                            restaurant.DeliveryRadius
                        );
                    }

                    // Store results
                    TempData["CustomerAddress"] = geocodingResponse[0].display_name;
                    TempData["CustomerLat"] = customerLat;
                    TempData["CustomerLng"] = customerLng;

                    // Serialize with error handling
                    try
                    {
                        var serializedRestaurants = JsonSerializer.Serialize(restaurantsInRange);
                        TempData["AvailableRestaurants"] = serializedRestaurants;
                        //_logger.LogInformation("Successfully serialized {Count} restaurants", restaurantsInRange.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error serializing restaurants");
                        TempData["Error"] = "Error processing restaurant data.";
                        return RedirectToAction("Index");
                    }

                    return RedirectToAction("Index", "Dashboard");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during restaurant query");
                    TempData["Error"] = "Error querying restaurants.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RedirectToDashboard");
                TempData["Error"] = "An error occurred while processing your request.";
                return RedirectToAction("Index");
            }
        }

        // Helper method to check if restaurant is in range
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

        private double ToRad(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        // Keep your existing actions...
        public IActionResult Index()
        {
            return View();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
