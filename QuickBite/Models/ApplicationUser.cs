using Microsoft.AspNetCore.Identity;

namespace QuickBite.Models;

public class ApplicationUser: IdentityUser
{
    public Guid? RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
}