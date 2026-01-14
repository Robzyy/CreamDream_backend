using CreamDream.Database;
using CreamDream.Database.Models;
using CreamDream.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace CreamDream.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly CreamDreamDbContext _context;
    private readonly ICartService _cartService;

    public OrderService(CreamDreamDbContext context, ICartService cartService)
    {
        _context = context;
        _cartService = cartService;
    }

    public async Task<Order> PlaceOrderAsync(int userId, int addressId, string? notes = null)
    {
        // verfica ca adresa exista si este a utlizatorului 
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

        if (address == null)
            throw new InvalidOperationException("Address not found or does not belong to user");

        // preia caruciorul cu iteme
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
            throw new InvalidOperationException("Cart is empty");

        // veridica daca toate produsele sunt in stoc
        foreach (var item in cart.CartItems)
        {
            if (item.Product.IsAvailable == 0)
                throw new InvalidOperationException($"Product '{item.Product.Name}' is no longer available");
        }

        // fa total
        var totalAmount = cart.CartItems.Sum(ci => ci.Product.Price * ci.Quantity);

        // genereaza order number  (format: ORD-YYYYMMDD-XXXXX)
        var orderNumber = await GenerateOrderNumberAsync();

        // creeaza comanda
        var order = new Order
        {
            UserId = userId,
            AddressId = addressId,
            OrderNumber = orderNumber,
            Status = "Pending",
            TotalAmount = totalAmount,
            OrderDate = DateTime.UtcNow.ToString("o"),
            Notes = notes
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // fa copie (snapshot) a preturilor
        foreach (var cartItem in cart.CartItems)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price,
                Subtotal = cartItem.Product.Price * cartItem.Quantity
            };

            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        // goleste caruciorul
        await _cartService.ClearCartAsync(userId);

        // refresh
        return await GetOrderByIdAsync(order.Id) ?? order;
    }

    public async Task<List<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId, int? userId = null)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .Include(o => o.User)
            .AsQueryable();

        // verifica daca utilizatorul exista (paote fi sters)
        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        return await query.FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<Order>> GetAllOrdersAsync(
        string? status = null, int? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(o => o.Status == status);

        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        if (fromDate.HasValue)
        {
            var fromDateStr = fromDate.Value.ToString("o");
            query = query.Where(o => string.Compare(o.OrderDate, fromDateStr) >= 0);
        }

        if (toDate.HasValue)
        {
            var toDateStr = toDate.Value.ToString("o");
            query = query.Where(o => string.Compare(o.OrderDate, toDateStr) <= 0);
        }

        return await query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> UpdateOrderStatusAsync(int orderId, string status)
    {
        // validare status
        var validStatuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };
        if (!validStatuses.Contains(status))
            throw new ArgumentException("Invalid status");

        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return null;

        order.Status = status;

        if (status == "Completed")
            order.CompletedAt = DateTime.UtcNow.ToString("o");

        await _context.SaveChangesAsync();

        // reload
        return await GetOrderByIdAsync(orderId);
    }

    public async Task<bool> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null)
            return false;

        // Can only cancel if not already completed or cancelled
        if (order.Status == "Completed" || order.Status == "Cancelled")
            return false;

        order.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<Dictionary<string, object>> GetOrderStatisticsAsync()
    {
        var totalOrders = await _context.Orders.CountAsync();
        var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
        var processingOrders = await _context.Orders.CountAsync(o => o.Status == "Processing");
        var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
        var cancelledOrders = await _context.Orders.CountAsync(o => o.Status == "Cancelled");
        
        var totalRevenue = await _context.Orders
            .Where(o => o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);

        var averageOrderValue = completedOrders > 0 
            ? totalRevenue / completedOrders 
            : 0;

        // toate comenzile din ziua curenta
        var today = DateTime.UtcNow.Date.ToString("o");
        var todayOrders = await _context.Orders
            .CountAsync(o => o.OrderDate.StartsWith(today.Substring(0, 10)));

        // banetu pe sapt actuala
        var weekAgo = DateTime.UtcNow.AddDays(-7).ToString("o");
        var weekRevenue = await _context.Orders
            .Where(o => o.Status == "Completed" && string.Compare(o.OrderDate, weekAgo) >= 0)
            .SumAsync(o => o.TotalAmount);

        return new Dictionary<string, object>
        {
            { "totalOrders", totalOrders },
            { "pendingOrders", pendingOrders },
            { "processingOrders", processingOrders },
            { "completedOrders", completedOrders },
            { "cancelledOrders", cancelledOrders },
            { "totalRevenue", totalRevenue },
            { "averageOrderValue", averageOrderValue },
            { "todayOrders", todayOrders },
            { "weekRevenue", weekRevenue }
        };
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"ORD-{datePart}-";

        // numere incrementale pemtru order number
        var lastOrder = await _context.Orders
            .Where(o => o.OrderNumber.StartsWith(prefix))
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        int nextNumber = 1;
        if (lastOrder != null)
        {
            var lastNumberPart = lastOrder.OrderNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumberPart, out var lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        return $"{prefix}{nextNumber:D5}";
    }
}
