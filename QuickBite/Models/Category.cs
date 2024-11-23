namespace QuickBite.Models;

public class Category
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    
    public List<Product>? Products { get; set; }
}