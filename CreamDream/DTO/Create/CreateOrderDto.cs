namespace CreamDream.DTO.Create;

public class CreateOrderDto
{
    public int CustomerId { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
}
