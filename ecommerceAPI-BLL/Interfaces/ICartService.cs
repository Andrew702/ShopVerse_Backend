using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;

namespace ecommerceAPI.BLL.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CartItemResponse>> GetCartAsync(string userId);
    Task<CartItemResponse> AddItemAsync(string userId, AddToCartRequest request);
    Task UpdateQuantityAsync(int cartItemId, string userId, UpdateCartItemRequest request);
    Task RemoveItemAsync(int cartItemId, string userId);
    Task ClearCartAsync(string userId);
}
