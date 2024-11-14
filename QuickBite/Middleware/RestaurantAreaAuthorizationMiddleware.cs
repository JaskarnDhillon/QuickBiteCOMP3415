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

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request is for the "Restaurant" area
        var endpoint = context.GetEndpoint();
        if (endpoint != null && endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()?.RouteValues["area"] == "Restaurant")
        {
            // Check if the "RestaurantId" session variable is set
            if (string.IsNullOrEmpty(context.Session.GetString("RestaurantId")))
            {
                // Redirect to the Unauthorized page if the session variable is missing
                context.Response.Redirect("/Home/Unauthorized");
                return;
            }
        }

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
