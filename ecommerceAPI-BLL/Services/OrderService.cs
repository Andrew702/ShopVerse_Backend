using AutoMapper;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Exceptions;
using ecommerceAPI.BLL.Interfaces;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Enums;
using ecommerceAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderResponse> CreateOrderAsync(string userId)
    {
        var cartItems = await _unitOfWork.CartItems.GetQueryable()
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        if (!cartItems.Any())
            throw new BadRequestException("Cart is empty. Add items before creating an order.");

        var order = new Order
        {
            UserId = userId,
            Total = cartItems.Sum(ci => ci.Product.Price * ci.Quantity),
            Date = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.CompleteAsync();

        foreach (var cartItem in cartItems)
        {
            await _unitOfWork.OrderItems.AddAsync(new OrderItem
            {
                OrderId = order.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Product.Price
            });
        }

        _unitOfWork.CartItems.DeleteRange(cartItems);
        await _unitOfWork.CompleteAsync();

        var fullOrder = await _unitOfWork.Orders.GetQueryable()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        return _mapper.Map<OrderResponse>(fullOrder!);
    }

    public async Task<IEnumerable<OrderResponse>> GetUserOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.Orders.GetQueryable()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return _mapper.Map<IEnumerable<OrderResponse>>(orders);
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetQueryable()
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order == null ? null : _mapper.Map<OrderResponse>(order);
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, string newStatus)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
            ?? throw new NotFoundException($"Order with ID {orderId} not found.");

        if (!Enum.TryParse<OrderStatus>(newStatus, true, out var status))
            throw new BadRequestException($"Invalid order status: {newStatus}.");

        order.Status = status;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<OrderResponse>(order);
    }
}
