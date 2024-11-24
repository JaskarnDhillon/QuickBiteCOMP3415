using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Models;

public class Restaurant
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }

    // Address information
    public string? Address { get; set; }

    // Latitude and Longitude for geolocation
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public int DeliveryRadius { get; set; }

    public DateTime OpeningHour {get;set;}

    public DateTime CloseingHour {get;set;}

    public bool isAccepted { get; set; }


    public string? RestaurantOwenrId  { get; set; }

    [ForeignKey(nameof(RestaurantOwenrId))]
    public ApplicationUser? RestaurantOwner  { get; set; }


    public List<Product>? Products { get; set; }

    public ICollection<Order>? Orders { get; set; }
    
}