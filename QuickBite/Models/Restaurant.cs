namespace QuickBite.Models;

public class Restaurant
{
    public Guid RestaurantId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }
    
    public List<Product>? Products { get; set; }
    
}