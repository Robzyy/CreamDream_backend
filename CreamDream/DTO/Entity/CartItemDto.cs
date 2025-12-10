namespace CreamDream.DTO.Entity;

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int AvailableStock { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime AddedAt { get; set; }
}
