using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuickBite.Models;

public class Order
{
    public Guid OrderId { get; set; }

    [Display(Name = "Order Date")]
    public DateTime OrderDate { get; set; }

    [Display(Name = "Order Total")]
    [DisplayFormat(DataFormatString = "{0:c}")]
    public decimal OrderTotal { get; set; }

    [Required]
    [Display(Name = "First Name")]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(255)]
    public string Address { get; set; }

    [Required]
    [MaxLength(50)]
    public string City { get; set; }

    [Required]
    [MaxLength(2)]
    public string Province { get; set; }

    [Required]
    [MaxLength(10)]
    [DataType(DataType.PostalCode)]
    [Display(Name = "Postal Code")]
    public string PostalCode { get; set; }

    [Required]
    [MaxLength(15)]
    public string Phone { get; set; }

    [Required]
    public Guid CustomerId { get; set; }
    
    public string? OrderNotes { get; set; }
    
    //enum for order status
    public OrderStatus OrderStatus { get; set; }

    // child ref
    public List<OrderDetail>? OrderDetails { get; set; }


    public Guid? RestaurantId { get; set; }

    [ForeignKey(nameof(RestaurantId))]
    public virtual Restaurant? Restaurant { get; set; }
}