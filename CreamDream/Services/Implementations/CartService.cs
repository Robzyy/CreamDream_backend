using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class CartService : ICartService
{
    private readonly CreamDreamDbContext _context;

    public CartService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.ProductType)
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow.ToString("o"),
                UpdatedAt = DateTime.UtcNow.ToString("o")
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id) ?? cart;
        }

        return cart;
    }

    public async Task<Cart> AddItemToCartAsync(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        // Verifica existenta produs
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Product not found");

        if (product.IsAvailable == 0)
            throw new InvalidOperationException("Product is not available");

        var cart = await GetOrCreateCartAsync(userId);

        // Verifica daca item-ul exista deja in cart
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

        if (existingItem != null)
        {
            // Daca da actualizaeaza cantitate
            existingItem.Quantity += quantity;
        }
        else
        {
            // daca nu, adauga-l
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity,
                AddedAt = DateTime.UtcNow.ToString("o")
            };

            _context.CartItems.Add(cartItem);
        }

        cart.UpdatedAt = DateTime.UtcNow.ToString("o");
        await _context.SaveChangesAsync();

        return await GetOrCreateCartAsync(userId);
    }

    public async Task<Cart?> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return null;

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (cartItem == null)
            return null;

        if (quantity <= 0)
        {
            // Sterge item daca cantitatea este 0
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity = quantity;
        }

        cart.UpdatedAt = DateTime.UtcNow.ToString("o");
        await _context.SaveChangesAsync();
        
        return await GetOrCreateCartAsync(userId);
    }

    public async Task<bool> RemoveItemFromCartAsync(int userId, int productId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return false;

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (cartItem == null)
            return false;

        _context.CartItems.Remove(cartItem);
        cart.UpdatedAt = DateTime.UtcNow.ToString("o");
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return false;

        _context.CartItems.RemoveRange(cart.CartItems);
        cart.UpdatedAt = DateTime.UtcNow.ToString("o");
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<double> GetCartTotalAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return 0;

        return cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);
    }

    public async Task<int> GetCartItemCountAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
            return 0;

        return cart.CartItems.Sum(ci => ci.Quantity);
    }
}
