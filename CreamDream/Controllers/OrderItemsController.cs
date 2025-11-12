using CreamDream.DTO.Create;
using CreamDream.DTO.Entity;
using CreamDream.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CreamDream.Controllers;

[ApiController]
[Route("api/orders/{orderId}/items")]
public class OrderItemsController : ControllerBase
{
    private readonly IOrderItemService _orderItemService;

    public OrderItemsController(IOrderItemService orderItemService)
    {
        _orderItemService = orderItemService;
    }

    /// <summary>
    /// Get all items for an order
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(int orderId)
    {
        var items = await _orderItemService.GetOrderItemsByOrderIdAsync(orderId);
        return Ok(items);
    }

    /// <summary>
    /// Get a specific order item by id
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderItemDto>> GetById(int id)
    {
        var item = await _orderItemService.GetOrderItemByIdAsync(id);
        if (item == null)
            return NotFound($"Order item with id {id} not found");

        return Ok(item);
    }

    /// <summary>
    /// Add an item to an order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create(int orderId, [FromBody] CreateOrderItemDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var item = await _orderItemService.CreateOrderItemAsync(createDto, orderId);
            return CreatedAtAction(nameof(GetById), new { orderId, id = item.Id }, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete an order item
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _orderItemService.DeleteOrderItemAsync(id);
        if (!success)
            return NotFound($"Order item with id {id} not found");

        return NoContent();
    }
}
