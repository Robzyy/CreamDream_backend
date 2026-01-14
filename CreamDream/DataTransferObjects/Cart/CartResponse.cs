namespace CreamDream.DataTransferObjects.Cart;

public class CartResponse
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public List<CartItemResponse> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
    public string UpdatedAt { get; set; } = null!;
}
