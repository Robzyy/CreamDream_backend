namespace CreamDream.DTO.Create;

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Category { get; set; } // 0=Donut, 1=Bagel, 2=Coffee
    public int Quantity { get; set; }
}
