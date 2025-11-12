using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.DTO.Update;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Get an order by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound($"Order with id {id} not found");

        return Ok(order);
    }

    /// <summary>
    /// Get all orders for a customer
    /// </summary>
    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByCustomerId(int customerId)
    {
        var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
        return Ok(orders);
    }

    /// <summary>
    /// Get orders by status (0=Pending, 1=Processing, 2=OutForDelivery, 3=Delivered, 4=Cancelled)
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetByStatus(int status)
    {
        var orders = await _orderService.GetOrdersByStatusAsync(status);
        return Ok(orders);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var order = await _orderService.CreateOrderAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Update an order
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] UpdateOrderDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var order = await _orderService.UpdateOrderAsync(id, updateDto);
        if (order == null)
            return NotFound($"Order with id {id} not found");

        return Ok(order);
    }

    /// <summary>
    /// Delete an order
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _orderService.DeleteOrderAsync(id);
        if (!success)
            return NotFound($"Order with id {id} not found");

        return NoContent();
    }
}
