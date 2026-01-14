using CreamDream.Database.Models;

namespace CreamDream.Services.Abstractions;

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(int userId, int addressId, string? notes = null);

    Task<List<Order>> GetUserOrdersAsync(int userId);

    Task<Order?> GetOrderByIdAsync(int orderId, int? userId = null);

    Task<List<Order>> GetAllOrdersAsync(string? status = null, int? userId = null, DateTime? fromDate = null, DateTime? toDate = null);

    Task<Order?> UpdateOrderStatusAsync(int orderId, string status);

    Task<bool> CancelOrderAsync(int orderId, int userId);

    Task<Dictionary<string, object>> GetOrderStatisticsAsync();
}
