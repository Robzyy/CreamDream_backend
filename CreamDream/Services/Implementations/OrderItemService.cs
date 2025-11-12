using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class OrderItemService : IOrderItemService
{
    private readonly CreamDreamDbContext _context;

    public OrderItemService(CreamDreamDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
    {
        var items = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Include(oi => oi.Product)
            .ToListAsync();
        return items.Select(MapToDto);
    }

    public async Task<OrderItemDto?> GetOrderItemByIdAsync(int id)
    {
        var item = await _context.OrderItems
            .Include(oi => oi.Product)
            .FirstOrDefaultAsync(oi => oi.Id == id);
        return item != null ? MapToDto(item) : null;
    }

    public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto createDto, int orderId)
    {
        var product = await _context.Products.FindAsync(createDto.ProductId);
        if (product == null)
            throw new ArgumentException($"Product with id {createDto.ProductId} not found");

        var orderItem = new OrderItem
        {
            OrderId = orderId,
            ProductId = createDto.ProductId,
            Quantity = createDto.Quantity,
            UnitPrice = product.Price
        };

        _context.OrderItems.Add(orderItem);

        // Update order total price
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.TotalPrice += product.Price * createDto.Quantity;
            _context.Orders.Update(order);
        }

        await _context.SaveChangesAsync();

        await _context.Entry(orderItem).Reference(oi => oi.Product).LoadAsync();

        return MapToDto(orderItem);
    }

    public async Task<bool> DeleteOrderItemAsync(int id)
    {
        var item = await _context.OrderItems.FindAsync(id);
        if (item == null)
            return false;

        // Update order total price
        var order = await _context.Orders.FindAsync(item.OrderId);
        if (order != null)
        {
            order.TotalPrice -= item.UnitPrice * item.Quantity;
            _context.Orders.Update(order);
        }

        _context.OrderItems.Remove(item);
        await _context.SaveChangesAsync();

        return true;
    }

    private OrderItemDto MapToDto(OrderItem item)
    {
        return new OrderItemDto
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Product = item.Product != null ? new ProductDto
            {
                Id = item.Product.Id,
                Name = item.Product.Name,
                Description = item.Product.Description,
                Price = item.Product.Price,
                Category = (int)item.Product.Category,
                Quantity = item.Product.Quantity,
                CreatedAt = item.Product.CreatedAt
            } : null
        };
    }
}
