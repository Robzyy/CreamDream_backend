using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;

namespace CreamDream.Services.Abstractions;

public interface ICartService
{
    Task<CartDto?> GetCartByCustomerIdAsync(int customerId);
    Task<CartItemDto> AddItemToCartAsync(int customerId, CreateCartItemDto dto);
    Task<CartItemDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemDto dto);
    Task RemoveItemFromCartAsync(int customerId, int cartItemId);
    Task ClearCartAsync(int customerId);
    Task<CartDto> ValidateCartStockAsync(int customerId);
}
