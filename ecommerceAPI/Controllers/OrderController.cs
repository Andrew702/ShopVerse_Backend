using System.Security.Claims;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var order = await _orderService.CreateOrderAsync(UserId);
        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _orderService.GetUserOrdersAsync(UserId);
        return Ok(orders);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId, UserId);
        return Ok(order);
    }

    [HttpPut("{orderId:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orderService.UpdateOrderStatusAsync(orderId, request.Status);
        return Ok(order);
    }
}
