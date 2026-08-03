using AutoMapper;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Exceptions;
using ecommerceAPI.BLL.Interfaces;
using ecommerceAPI.DAL.Entities;
using ecommerceAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.BLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(
        int page, int pageSize,
        string? search, int? categoryId, int? brandId,
        decimal? minPrice, decimal? maxPrice, double? minRating,
        bool? onSale, bool? inStock, string? sortBy)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .AsQueryable();

        // ── Filters ──────────────────────────────────
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (minRating.HasValue)
        {
            query = query.Where(p =>
                p.Reviews.Any()
                    ? p.Reviews.Average(r => r.Rating) >= (decimal)minRating.Value
                    : false);
        }

        if (onSale.HasValue)
            query = query.Where(p => p.IsOnSale == onSale.Value);

        if (inStock.HasValue)
        {
            query = inStock.Value
                ? query.Where(p => p.StockQuantity > 0)
                : query.Where(p => p.StockQuantity == 0);
        }

        // ── Sort ─────────────────────────────────────
        query = sortBy switch
        {
            "price-asc"  => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "rating"     => query.OrderByDescending(p =>
                p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0m),
            "name-asc"   => query.OrderBy(p => p.Title),
            "name-desc"  => query.OrderByDescending(p => p.Title),
            _            => query
        };

        // ── Paginate ─────────────────────────────────
        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<ProductResponse>
        {
            Items = _mapper.Map<List<ProductResponse>>(products),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDetailResponse> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new NotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductDetailResponse>(product);
    }

    public async Task<ReviewResponse> AddReviewAsync(int productId, string reviewerName, CreateReviewRequest request)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product with ID {productId} not found.");

        var review = new Review
        {
            ProductId = productId,
            Rating = request.Rating,
            Comment = request.Comment,
            ReviewerName = reviewerName,
            Date = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ReviewResponse>(review);
    }

    public async Task<IEnumerable<ProductResponse>> GetByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<IEnumerable<ProductResponse>> GetByBrandAsync(int brandId)
    {
        var products = await _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .Where(p => p.BrandId == brandId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<IEnumerable<ProductResponse>> SearchAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .Where(p => p.Title.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    public async Task<IEnumerable<BrandResponse>> GetAllBrandsAsync()
    {
        var brands = await _unitOfWork.Brands.GetAllAsync();
        return _mapper.Map<IEnumerable<BrandResponse>>(brands);
    }
}
