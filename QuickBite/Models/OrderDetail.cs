using System.ComponentModel.DataAnnotations;

namespace QuickBite.Models;

public class OrderDetail
{
    public Guid OrderDetailId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    // parent refs
    public Product? Product { get; set; }
    public Order? Order { get; set; }
}