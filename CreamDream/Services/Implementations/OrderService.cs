using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly CreamDreamDbContext _context;

    public OrderService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
        return order != null ? MapToDto(order) : null;
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(int customerId)
    {
        var orders = await _context.Orders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        return orders.Select(MapToDto);
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(int status)
    {
        var orders = await _context.Orders
            .Where(o => (int)o.Status == status)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ToListAsync();
        return orders.Select(MapToDto);
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createDto)
    {
        var order = new Order
        {
            CustomerId = createDto.CustomerId,
            DeliveryAddress = createDto.DeliveryAddress,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            TotalPrice = 0 // Will be calculated
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order items
        decimal totalPrice = 0;
        foreach (var itemDto in createDto.OrderItems)
        {
            var product = await _context.Products.FindAsync(itemDto.ProductId);
            if (product != null)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };

                totalPrice += product.Price * itemDto.Quantity;
                _context.OrderItems.Add(orderItem);
            }
        }

        order.TotalPrice = totalPrice;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        // Reload with items
        await _context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
        foreach (var item in order.OrderItems)
        {
            await _context.Entry(item).Reference(oi => oi.Product).LoadAsync();
        }

        return MapToDto(order);
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateDto)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return null;

        order.Status = (OrderStatus)updateDto.Status;
        order.DeliveryAddress = updateDto.DeliveryAddress;
        order.DeliveryDate = updateDto.DeliveryDate;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        await _context.Entry(order).Collection(o => o.OrderItems).LoadAsync();
        foreach (var item in order.OrderItems)
        {
            await _context.Entry(item).Reference(oi => oi.Product).LoadAsync();
        }

        return MapToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return false;

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            TotalPrice = order.TotalPrice,
            Status = (int)order.Status,
            DeliveryAddress = order.DeliveryAddress,
            DeliveryDate = order.DeliveryDate,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Product = oi.Product != null ? new ProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    Description = oi.Product.Description,
                    Price = oi.Product.Price,
                    Category = (int)oi.Product.Category,
                    Quantity = oi.Product.Quantity,
                    CreatedAt = oi.Product.CreatedAt
                } : null
            }).ToList()
        };
    }
}
