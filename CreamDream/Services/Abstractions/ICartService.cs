using CreamDream.Database.Models;

namespace CreamDream.Services.Abstractions;

public interface ICartService
{
    Task<Cart> GetOrCreateCartAsync(int userId);

    Task<Cart> AddItemToCartAsync(int userId, int productId, int quantity);

    Task<Cart?> UpdateCartItemQuantityAsync(int userId, int productId, int quantity);

    Task<bool> RemoveItemFromCartAsync(int userId, int productId);

    Task<bool> ClearCartAsync(int userId);

    Task<double> GetCartTotalAsync(int userId);

    Task<int> GetCartItemCountAsync(int userId);
}
