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
     
        public async Task<IActionResult> RedirectToDashboard(string address)
        {


            return RedirectToAction("Index", "Dashboard", new { address = address });
        }

        // Helper method to check if restaurant is in range
      

        

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
