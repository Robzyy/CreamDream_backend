using CreamDream.Database.Models;

namespace CreamDream.Services.Abstractions;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync(int? productTypeId = null, int? categoryId = null, bool? isAvailable = true);

    Task<Product?> GetProductByIdAsync(int productId);

    Task<List<ProductType>> GetProductTypesAsync();

    Task<List<Category>> GetCategoriesAsync();

    Task<Product> CreateProductAsync(string name, string? description, double price, int productTypeId, int? categoryId = null, string? imageUrl = null);

    Task<Product?> UpdateProductAsync(int productId, string name, string? description, double price, int productTypeId, int? categoryId = null, string? imageUrl = null);

    Task<bool> DeleteProductAsync(int productId);

    Task<bool> ToggleProductAvailabilityAsync(int productId, bool isAvailable);

    Task<List<Product>> SearchProductsAsync(string searchTerm, bool? isAvailable = true);
}
