using System.ComponentModel.DataAnnotations;

namespace QuickBite.Models;

public class Product
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }

    [DisplayFormat(DataFormatString = "{0:c}")]
    public decimal Price { get; set; }
        
    public Guid CategoryId { get; set; }
    public Guid RestaurantId { get; set; }
    public string? Description { get; set; }

    public string? Photo { get; set; }
    
    public Category? Category { get; set; }
    public Restaurant? Restaurant { get; set; }
    
    public List<CartItem>? CartItems { get; set; }
    public List<OrderDetail>? OrderDetails { get; set; }
}