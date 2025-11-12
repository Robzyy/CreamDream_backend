using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;

namespace CreamDream.Services.Abstractions;

public interface IOrderItemService
{
    Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId);
    Task<OrderItemDto?> GetOrderItemByIdAsync(int id);
    Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto, int orderId);
    Task<bool> DeleteOrderItemAsync(int id);
}
