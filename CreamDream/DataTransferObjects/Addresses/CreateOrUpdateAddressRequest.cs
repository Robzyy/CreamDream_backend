using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Addresses;

public class CreateOrUpdateAddressRequest
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100, ErrorMessage = "Full name must not exceed 100 characters")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters")]
    public string PhoneNumber { get; set; } = null!;

    [Required(ErrorMessage = "Street address is required")]
    [StringLength(200, ErrorMessage = "Street address must not exceed 200 characters")]
    public string StreetAddress { get; set; } = null!;

    [Required(ErrorMessage = "City is required")]
    [StringLength(100, ErrorMessage = "City must not exceed 100 characters")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "Postal code is required")]
    [StringLength(20, ErrorMessage = "Postal code must not exceed 20 characters")]
    public string PostalCode { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Country must not exceed 100 characters")]
    public string Country { get; set; } = "Romania";

    [StringLength(500, ErrorMessage = "Address notes must not exceed 500 characters")]
    public string? AddressNotes { get; set; }
}
