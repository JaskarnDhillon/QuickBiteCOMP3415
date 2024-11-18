using QuickBite.Data;

namespace QuickBite.Middleware;

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class RestaurantAreaAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RestaurantAreaAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceScopeFactory serviceScopeFactory)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint != null && endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.RouteValues["area"] == "Restaurant")
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Check if the session variable "RestaurantId" exists
                if (string.IsNullOrEmpty(context.Session.GetString("RestaurantId")))
                {
                    context.Response.Redirect("/Home/Unauthorized");
                    return;
                }
                
                var restaurantId = Guid.Parse(context.Session.GetString("RestaurantId"));
                var restaurant = await dbContext.Restaurant.FindAsync(restaurantId);
                if (!restaurant.isAccepted)
                {
                    context.Response.Redirect("/Home/Unauthorized");
                    return;
                }
            }
        }

        // Proceed to the next middleware
        await _next(context);
    }
}
