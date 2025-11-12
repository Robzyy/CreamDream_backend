using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;

namespace CreamDream.Services.Abstractions;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId);
    Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(int status);
    Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto);
    Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto);
    Task<bool> DeleteOrderAsync(int id);
}
