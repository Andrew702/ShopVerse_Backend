namespace ecommerceAPI.BLL.DTOs.Response;

public class AuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public List<int> Wishlist { get; set; } = new();
    public List<CartItemResponse> CartItems { get; set; } = new();
    public List<OrderResponse> Orders { get; set; } = new();
}
