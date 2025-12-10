namespace CreamDream.DTO.Create;

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public int CartId { get; set; }
    public string? DeliveryAddress { get; set; }
}
