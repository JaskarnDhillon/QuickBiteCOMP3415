using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickBite.Models;
using System.Diagnostics;
using static QuickBite.Controllers.HomeController;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using QuickBite.Data;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
          ILogger<HomeController> logger,
          ApplicationDbContext context,
          UserManager<ApplicationUser> userManager,
          IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "QuickBite-RestaurantApp");
            _userManager = userManager;
        }

        public class DashboardIndexViewModel()
        {
           public List<Restaurant> Restaurants {  get; set; }
        }
        [Route("/dashboard")]
        public async Task<IActionResult> Index(string address)
        {
            if(address == null)
            {
                if(HttpContext.Session.GetString("CustomerAddress") != null)
                {
                    address = HttpContext.Session.GetString("CustomerAddress");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

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
            
            HttpContext.Session.SetString("CustomerAddress", address);
            ViewBag.CustomerAddress = address;
            
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
        public IActionResult Orders(Guid customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToList();
            
            return View(orders);
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
        public async Task<IActionResult> Settings(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            return View(user);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string Id, [Bind("FirstName, LastName, PhoneNumber, Id")] ApplicationUser user)
        {
            _context.Update(user);
            return RedirectToAction("Index");
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

        public async Task<IActionResult> AddToCart(Guid ProductId, int Quantity)
        {
            var product = await _context.Products.FindAsync(ProductId);
            
            if (product == null)
            {
                return NotFound();
            }

            var customerId = GetCustomerId();
            
            var cartItem = _context.CartItems
                .Where(c => c.ProductId == ProductId && c.CustomerId == Guid.Parse((customerId)))
                .FirstOrDefault();

            
            if (cartItem == null)
            {
                // create a new product object
                cartItem = new CartItem
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                    CustomerId = Guid.Parse(customerId),
                    Price = product.Price
                };
               
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += Quantity;
                _context.CartItems.Update(cartItem);
            }

            // save to db
            _context.SaveChanges();

            // redirect to cart page
            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            // identify the cart using the session var
            var customerId = GetCustomerId();

            // fetch the items in this cart
            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == Guid.Parse(customerId));

            // get total # of items in user's cart & store in session var for navbar badge
            int itemCount = (from c in cartItems
                select c.Quantity).Sum();

            HttpContext.Session.SetInt32("ItemCount", itemCount);

            return View(cartItems);
        }
        
        public IActionResult RemoveFromCart(Guid cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }
        
        [Authorize]
        public IActionResult Checkout()
        {
            ViewBag.CustomerId = GetCustomerId();
            return View();
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone,OrderNotes")] Order order)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            
            // auto-fill the other 3 order properties (date, total, customer)
            order.OrderDate = DateTime.Now.ToUniversalTime();
            order.CustomerId = Guid.Parse(customerId);

            // calc order total
            var cartItems = _context.CartItems.Where(c => c.CustomerId == order.CustomerId)
                .Include(c => c.Product);

            // get the restaurant from the product 

            var restaurant = _context.Restaurant.Where(r=>r.Products.Contains(cartItems.FirstOrDefault().Product)).FirstOrDefault();

            // save the restaurant Id of the order
            order.RestaurantId = restaurant.RestaurantId;

            order.OrderTotal = (from c in cartItems
                select (c.Quantity * c.Price)).Sum();

            order.OrderStatus = OrderStatus.Placed;
            
            // save order to db
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    OrderId = order.OrderId
                };

                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges(); 

            // empty user's cart
            foreach (var item in cartItems)
            {
                _context.CartItems.Remove(item);
            }
            _context.SaveChanges();
            
            return RedirectToAction("OrderDetails" , new { orderId = order.OrderId });
        }

        public IActionResult OrderDetails(Guid orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault();

            if (order.OrderStatus != OrderStatus.Delivered)
            {
                ViewBag.Delivered = false;
            }
            
            return View(order);
        }
        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmOrder(Guid orderId)
        {
            var order = _context.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault();
            
            order.OrderStatus = OrderStatus.Confirmed;
            _context.Orders.Update(order);
            _context.SaveChanges();
            
            return RedirectToAction("OrderDetails", new { orderId = orderId });
        }
        
        public string GetCustomerId()
        {
            // check for existing session var for this user
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                // if there is no session var, create one using a GUID
                HttpContext.Session.SetString("CustomerId", Guid.NewGuid().ToString());
            }

            // return the session var
            return HttpContext.Session.GetString("CustomerId");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}