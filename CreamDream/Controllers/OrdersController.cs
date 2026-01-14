using CreamDream.DataTransferObjects.Orders;
using CreamDream.DataTransferObjects.Addresses;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CreamDream.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IAddressService _addressService;

    public OrdersController(IOrderService orderService, IAddressService addressService)
    {
        _orderService = orderService;
        _addressService = addressService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var order = await _orderService.PlaceOrderAsync(userId, request.AddressId, request.Notes);

            var response = await MapOrderToResponse(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-orders")]
    public async Task<ActionResult<List<OrderResponse>>> GetMyOrders()
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var orders = await _orderService.GetUserOrdersAsync(userId);
            var response = new List<OrderResponse>();

            foreach (var order in orders)
            {
                response.Add(await MapOrderToResponse(order));
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
    {
        var userId = GetUserIdFromClaims();
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        try
        {
            // If user is not admin, only allow viewing their own orders
            var order = userRole == "Admin" 
                ? await _orderService.GetOrderByIdAsync(id)
                : await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
                return NotFound(new { message = "Order not found" });

            var response = await MapOrderToResponse(order);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAllOrders(
        [FromQuery] string? status = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(status, userId, fromDate, toDate);
            var response = new List<OrderResponse>();

            foreach (var order in orders)
            {
                response.Add(await MapOrderToResponse(order));
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);
            if (order == null)
                return NotFound(new { message = "Order not found" });

            var response = await MapOrderToResponse(order);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderResponse>> CancelOrder(int id)
    {
        var userId = GetUserIdFromClaims();

        try
        {
            var success = await _orderService.CancelOrderAsync(id, userId);
            if (!success)
                return BadRequest(new { message = "Cannot cancel this order" });

            var order = await _orderService.GetOrderByIdAsync(id, userId);
            var response = await MapOrderToResponse(order!);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("statistics")]
    public async Task<ActionResult<OrderStatisticsResponse>> GetStatistics()
    {
        try
        {
            var stats = await _orderService.GetOrderStatisticsAsync();

            var response = new OrderStatisticsResponse
            {
                TotalOrders = (int)stats["totalOrders"],
                PendingOrders = (int)stats["pendingOrders"],
                ProcessingOrders = (int)stats["processingOrders"],
                CompletedOrders = (int)stats["completedOrders"],
                CancelledOrders = (int)stats["cancelledOrders"],
                TotalRevenue = (decimal)(double)stats["totalRevenue"],
                AverageOrderValue = (decimal)(double)stats["averageOrderValue"],
                TodayOrders = (int)stats["todayOrders"],
                WeekRevenue = (decimal)(double)stats["weekRevenue"]
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task<OrderResponse> MapOrderToResponse(Database.Models.Order order)
    {
        AddressResponse? addressResponse = null;
        if (order.AddressId.HasValue)
        {
            var address = await _addressService.GetUserAddressAsync(order.UserId);
            if (address != null)
            {
                addressResponse = new AddressResponse
                {
                    Id = address.Id,
                    UserId = address.UserId,
                    FullName = address.FullName,
                    PhoneNumber = address.PhoneNumber,
                    StreetAddress = address.StreetAddress,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Country = address.Country,
                    AddressNotes = address.AddressNotes
                };
            }
        }

        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            TotalAmount = (decimal)order.TotalAmount,
            OrderDate = order.OrderDate,
            CompletedAt = order.CompletedAt,
            Notes = order.Notes,
            User = new OrderUserInfo
            {
                Id = order.User.Id,
                Username = order.User.Username,
                Email = order.User.Email
            },
            Address = addressResponse,
            Items = order.OrderItems.Select(oi => new OrderItemResponse
            {
                ProductId = oi.ProductId,
                ProductName = oi.ProductName,
                Quantity = oi.Quantity,
                UnitPrice = (decimal)oi.UnitPrice,
                Subtotal = (decimal)oi.Subtotal
            }).ToList()
        };
    }

    private int GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim!);
    }
}
