using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;

namespace CreamDream.Services.Abstractions;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int category);
    Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto);
    Task<bool> DeleteProductAsync(int id);
}
