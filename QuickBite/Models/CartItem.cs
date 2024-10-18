using System.ComponentModel.DataAnnotations;

namespace QuickBite.Models;

public class CartItem
{
    public Guid CartItemId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    // parent ref
    public Product? Product { get; set; }
}