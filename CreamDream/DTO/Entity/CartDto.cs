namespace CreamDream.DTO.Entity;

public class CartDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
