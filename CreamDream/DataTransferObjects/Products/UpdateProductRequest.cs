using System.ComponentModel.DataAnnotations;

namespace CreamDream.DataTransferObjects.Products;

public class UpdateProductRequest
{
    [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters")]
    public string? Name { get; set; }

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    [Range(0.01, 10000, ErrorMessage = "Price must be between 0.01 and 10000")]
    public decimal? Price { get; set; }

    public int? ProductTypeId { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
    public string? ImageUrl { get; set; }
}
