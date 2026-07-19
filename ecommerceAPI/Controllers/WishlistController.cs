using System.Security.Claims;
using ecommerceAPI.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var items = await _wishlistService.GetWishlistAsync(UserId);
        return Ok(items);
    }

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> AddToWishlist(int productId)
    {
        await _wishlistService.AddToWishlistAsync(UserId, productId);
        return Ok(new { message = "Added to wishlist." });
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        await _wishlistService.RemoveFromWishlistAsync(UserId, productId);
        return NoContent();
    }
}
