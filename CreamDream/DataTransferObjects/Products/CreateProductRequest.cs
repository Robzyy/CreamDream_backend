using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Products;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Product type is required")]
    public int ProductTypeId { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
    public string? ImageUrl { get; set; }
}
