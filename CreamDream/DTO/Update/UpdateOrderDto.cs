namespace CreamDream.DTO.Update;

public class UpdateOrderDto
{
    public int Status { get; set; } // 0=Pending, 1=Processing, 2=OutForDelivery, 3=Delivered, 4=Cancelled
    public string DeliveryAddress { get; set; } = string.Empty;
    public DateTime? DeliveryDate { get; set; }
}
