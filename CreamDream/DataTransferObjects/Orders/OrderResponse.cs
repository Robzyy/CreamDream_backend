using CreamDream.DataTransferObjects.Addresses;

namespace CreamDream.DataTransferObjects.Orders;

public class OrderResponse
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string OrderDate { get; set; } = null!;
    public string? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public OrderUserInfo User { get; set; } = null!;
    public AddressResponse? Address { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderUserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}
