namespace CreamDream.DataTransferObjects.Cart;

public class CartItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal ProductPrice { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public bool IsAvailable { get; set; }
}
