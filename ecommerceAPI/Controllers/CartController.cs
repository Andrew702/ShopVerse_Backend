using System.Security.Claims;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var items = await _cartService.GetCartAsync(UserId);
        return Ok(items);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
    {
        var item = await _cartService.AddItemAsync(UserId, request);
        return Ok(item);
    }

    [HttpPut("items/{cartItemId:int}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] UpdateCartItemRequest request)
    {
        await _cartService.UpdateQuantityAsync(cartItemId, UserId, request);
        return NoContent();
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        await _cartService.RemoveItemAsync(cartItemId, UserId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(UserId);
        return NoContent();
    }
}
