namespace CreamDream.DataTransferObjects.Products;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; }
    public ProductTypeResponse ProductType { get; set; } = null!;
    public CategoryResponse? Category { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string UpdatedAt { get; set; } = null!;
}
