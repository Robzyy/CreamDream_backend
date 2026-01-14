using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Orders;

public class PlaceOrderRequest
{
    [Required(ErrorMessage = "Address is required")]
    public int AddressId { get; set; }

    [StringLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
    public string? Notes { get; set; }
}
