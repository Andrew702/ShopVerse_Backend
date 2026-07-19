using ecommerceAPI.BLL.DTOs.Response;

namespace ecommerceAPI.BLL.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<WishlistResponse>> GetWishlistAsync(string userId);
    Task AddToWishlistAsync(string userId, int productId);
    Task RemoveFromWishlistAsync(string userId, int productId);
    Task<bool> IsInWishlistAsync(string userId, int productId);
}
