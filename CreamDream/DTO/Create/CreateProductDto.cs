namespace CreamDream.DTO.Create;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Category { get; set; }
    public int Quantity { get; set; }
}
