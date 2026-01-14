using CreamDream.Database;
using CreamDream.Database.Models;
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

    public async Task<List<Product>> GetProductsAsync(int? productTypeId = null, int? categoryId = null, bool? isAvailable = true)
    {
        var query = _context.Products
            .Include(p => p.ProductType)
            .Include(p => p.Category)
            .AsQueryable();

        // Verificaa Filtre
        if (productTypeId.HasValue)
            query = query.Where(p => p.ProductTypeId == productTypeId.Value);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isAvailable.HasValue)
            query = query.Where(p => p.IsAvailable == (isAvailable.Value ? 1 : 0));

        return await query
            .OrderBy(p => p.ProductTypeId)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _context.Products
            .Include(p => p.ProductType)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<List<ProductType>> GetProductTypesAsync()
    {
        return await _context.ProductTypes
            .OrderBy(pt => pt.Name)
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(
        string name, string? description, double price, int productTypeId, int? categoryId = null, string? imageUrl = null)
    {
        var productType = await _context.ProductTypes.FindAsync(productTypeId);
        if (productType == null)
            throw new InvalidOperationException("Product type not found");

        if (categoryId.HasValue)
        {
            var category = await _context.Categories.FindAsync(categoryId.Value);
            if (category == null)
                throw new InvalidOperationException("Category not found");
        }

        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            ProductTypeId = productTypeId,
            CategoryId = categoryId,
            ImageUrl = imageUrl,
            IsAvailable = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id) ?? product;
    }

    public async Task<Product?> UpdateProductAsync(
        int productId, string name, string? description, double price, int productTypeId, int? categoryId = null, string? imageUrl = null)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return null;

        var productType = await _context.ProductTypes.FindAsync(productTypeId);
        if (productType == null)
            throw new InvalidOperationException("Product type not found");

        if (categoryId.HasValue)
        {
            var category = await _context.Categories.FindAsync(categoryId.Value);
            if (category == null)
                throw new InvalidOperationException("Category not found");
        }

        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.ProductTypeId = productTypeId;
        product.CategoryId = categoryId;
        product.ImageUrl = imageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        var inCart = await _context.CartItems.AnyAsync(ci => ci.ProductId == productId);
        if (inCart)
            throw new InvalidOperationException("Cannot delete product that is in customer carts");

        var inOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);
        if (inOrders)
            throw new InvalidOperationException("Cannot delete product that appears in order history");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleProductAvailabilityAsync(int productId, bool isAvailable)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        product.IsAvailable = isAvailable ? 1 : 0;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm, bool? isAvailable = true)
    {
        var query = _context.Products
            .Include(p => p.ProductType)
            .Include(p => p.Category)
            .AsQueryable();

        if (isAvailable.HasValue)
            query = query.Where(p => p.IsAvailable == (isAvailable.Value ? 1 : 0));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerSearch) ||
                (p.Description != null && p.Description.ToLower().Contains(lowerSearch)));
        }

        return await query
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
