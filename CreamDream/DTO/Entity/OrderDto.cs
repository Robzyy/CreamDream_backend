namespace CreamDream.DTO.Entity;

public class OrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public int Status { get; set; } // 0=Pending, 1=Processing, 2=OutForDelivery, 3=Delivered, 4=Cancelled
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
    public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
}
