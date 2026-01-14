using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Orders;

public class UpdateOrderStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    [RegularExpression("^(Pending|Processing|Completed|Cancelled)$", 
        ErrorMessage = "Status must be one of: Pending, Processing, Completed, Cancelled")]
    public string Status { get; set; } = null!;
}
