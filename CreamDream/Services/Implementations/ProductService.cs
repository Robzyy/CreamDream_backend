using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class ProductService : IProductService
{
    private readonly CreamDreamDbContext _context;

    public ProductService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int category)
    {
        var products = await _context.Products
            .Where(p => (int)p.Category == category)
            .ToListAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Category = (ProductCategory)createDto.Category,
            Quantity = createDto.Quantity
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return null;

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.Category = (ProductCategory)updateDto.Category;
        product.Quantity = updateDto.Quantity;

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = (int)product.Category,
            Quantity = product.Quantity,
            CreatedAt = product.CreatedAt
        };
    }
}
