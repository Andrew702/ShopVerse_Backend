using AutoMapper;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Exceptions;
using ecommerceAPI.BLL.Interfaces;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.BLL.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartItemResponse>> GetCartAsync(string userId)
    {
        var cartItems = await _unitOfWork.CartItems.GetQueryable()
            .Include(ci => ci.Product)
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CartItemResponse>>(cartItems);
    }

    public async Task<CartItemResponse> AddItemAsync(string userId, AddToCartRequest request)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId)
            ?? throw new NotFoundException($"Product with ID {request.ProductId} not found.");

        var existingItem = await _unitOfWork.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == request.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
            _unitOfWork.CartItems.Update(existingItem);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<CartItemResponse>(existingItem);
        }

        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        await _unitOfWork.CartItems.AddAsync(cartItem);
        await _unitOfWork.CompleteAsync();

        // Reload with product for response mapping
        var savedItem = await _unitOfWork.CartItems.GetQueryable()
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == cartItem.Id);

        return _mapper.Map<CartItemResponse>(savedItem!);
    }

    public async Task UpdateQuantityAsync(int cartItemId, string userId, UpdateCartItemRequest request)
    {
        var cartItem = await _unitOfWork.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId)
            ?? throw new NotFoundException($"Cart item with ID {cartItemId} not found.");

        cartItem.Quantity = request.Quantity;
        _unitOfWork.CartItems.Update(cartItem);
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveItemAsync(int cartItemId, string userId)
    {
        var cartItem = await _unitOfWork.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId)
            ?? throw new NotFoundException($"Cart item with ID {cartItemId} not found.");

        _unitOfWork.CartItems.Delete(cartItem);
        await _unitOfWork.CompleteAsync();
    }

    public async Task ClearCartAsync(string userId)
    {
        var cartItems = await _unitOfWork.CartItems
            .FindAsync(ci => ci.UserId == userId);

        _unitOfWork.CartItems.DeleteRange(cartItems);
        await _unitOfWork.CompleteAsync();
    }
}
