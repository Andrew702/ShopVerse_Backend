using AutoMapper;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Exceptions;
using ecommerceAPI.BLL.Interfaces;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.BLL.Services;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WishlistResponse>> GetWishlistAsync(string userId)
    {
        var wishlist = await _unitOfWork.Wishlists.GetQueryable()
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<WishlistResponse>>(wishlist);
    }

    public async Task AddToWishlistAsync(string userId, int productId)
    {
        var existing = await _unitOfWork.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        if (existing != null)
            return;

        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product with ID {productId} not found.");

        await _unitOfWork.Wishlists.AddAsync(new Wishlist
        {
            UserId = userId,
            ProductId = productId
        });
        await _unitOfWork.CompleteAsync();
    }

    public async Task RemoveFromWishlistAsync(string userId, int productId)
    {
        var item = await _unitOfWork.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId)
            ?? throw new NotFoundException("Wishlist item not found.");

        _unitOfWork.Wishlists.Delete(item);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<bool> IsInWishlistAsync(string userId, int productId)
    {
        return await _unitOfWork.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
    }
}
