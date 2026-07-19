using ecommerceAPI.BLL.DTOs.Response;

namespace ecommerceAPI.BLL.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(string userId);
    Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId);
    Task<OrderResponse?> GetOrderByIdAsync(int orderId);
    Task<OrderResponse> UpdateOrderStatusAsync(int orderId, string newStatus);
}
