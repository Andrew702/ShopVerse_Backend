using AutoMapper;
using ecommerceAPI.BLL.DTOs.Response;
using ecommerceAPI.BLL.Interfaces;
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

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(int page, int pageSize,
        string? search, int? categoryId, int? brandId)
    {
        var query = _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

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

    public async Task<ProductDetailResponse?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetQueryable()
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null ? null : _mapper.Map<ProductDetailResponse>(product);
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
