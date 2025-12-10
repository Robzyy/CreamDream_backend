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
        // Load cart with items
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.Id == createDto.CartId);

        if (cart == null)
            throw new InvalidOperationException($"Cart with ID {createDto.CartId} not found.");

        if (!cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty. Cannot create order from empty cart.");

        // Get customer to use their address if not provided
        var customer = await _context.Customers.FindAsync(createDto.CustomerId);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {createDto.CustomerId} not found.");

        // Create order
        var deliveryAddress = !string.IsNullOrWhiteSpace(createDto.DeliveryAddress) 
            ? createDto.DeliveryAddress 
            : customer.Address;

        var order = new Order
        {
            CustomerId = createDto.CustomerId,
            DeliveryAddress = deliveryAddress,
            Status = OrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            TotalPrice = 0 // Will be calculated
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order items from cart
        decimal totalPrice = 0;
        foreach (var cartItem in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Price
            };

            totalPrice += cartItem.Price * cartItem.Quantity;
            _context.OrderItems.Add(orderItem);
        }

        order.TotalPrice = totalPrice;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        // Clear the cart
        _context.CartItems.RemoveRange(cart.CartItems);
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
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };
    }
}
