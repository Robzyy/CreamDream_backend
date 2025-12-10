using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class CartService : ICartService
{
    private readonly CreamDreamDbContext _context;
    private readonly IProductService _productService;

    public CartService(CreamDreamDbContext context, IProductService productService)
    {
        _context = context;
        _productService = productService;
    }

    public async Task<CartDto?> GetCartByCustomerIdAsync(int customerId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
            return null;

        return MapToCartDto(cart);
    }

    public async Task<CartItemDto> AddItemToCartAsync(int customerId, CreateCartItemDto dto)
    {
        var product = await _productService.GetProductByIdAsync(dto.ProductId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {dto.ProductId} not found.");

        if (product.Quantity < dto.Quantity)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {product.Quantity}, Requested: {dto.Quantity}");

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
        {
            cart = new Cart { CustomerId = customerId, CreatedAt = DateTime.UtcNow };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            if (product.Quantity < existingItem.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock. Available: {product.Quantity}, Requested total: {existingItem.Quantity}");
        }
        else
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = product.Price,
                AddedAt = DateTime.UtcNow
            };
            _context.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var updatedCart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == cart.Id);

        var addedItem = updatedCart!.CartItems.First(ci => ci.ProductId == dto.ProductId);
        return MapToCartItemDto(addedItem);
    }

    public async Task<CartItemDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
            throw new InvalidOperationException("Cart not found.");

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
            throw new InvalidOperationException("Cart item not found.");

        var product = cartItem.Product;
        if (product.Quantity < dto.Quantity)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {product.Quantity}, Requested: {dto.Quantity}");

        cartItem.Quantity = dto.Quantity;
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToCartItemDto(cartItem);
    }

    public async Task RemoveItemFromCartAsync(int customerId, int cartItemId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
            throw new InvalidOperationException("Cart not found.");

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (cartItem == null)
            throw new InvalidOperationException("Cart item not found.");

        _context.CartItems.Remove(cartItem);
        cart.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int customerId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.CartItems);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<CartDto> ValidateCartStockAsync(int customerId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (cart == null)
            throw new InvalidOperationException("Cart not found.");

        foreach (var item in cart.CartItems)
        {
            if (item.Product.Quantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Product '{item.Product.Name}' has insufficient stock. Available: {item.Product.Quantity}, In cart: {item.Quantity}");
        }

        return MapToCartDto(cart);
    }

    private CartDto MapToCartDto(Cart cart)
    {
        var items = cart.CartItems.Select(MapToCartItemDto).ToList();
        return new CartDto
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            Items = items,
            TotalPrice = items.Sum(i => i.Price * i.Quantity),
            CreatedAt = cart.CreatedAt,
            UpdatedAt = cart.UpdatedAt
        };
    }

    private CartItemDto MapToCartItemDto(CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.Product.Name,
            Quantity = cartItem.Quantity,
            Price = cartItem.Price,
            AvailableStock = cartItem.Product.Quantity,
            IsLowStock = cartItem.Product.Quantity < 20,
            AddedAt = cartItem.AddedAt
        };
    }
}
