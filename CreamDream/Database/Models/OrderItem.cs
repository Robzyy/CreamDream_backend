namespace CreamDream.Database.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Foreign key and navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
