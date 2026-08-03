using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.DTOs.Response;

namespace ecommerceAPI.BLL.Interfaces;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(
        int page, int pageSize,
        string? search, int? categoryId, int? brandId,
        decimal? minPrice, decimal? maxPrice, double? minRating,
        bool? onSale, bool? inStock, string? sortBy);
    Task<ProductDetailResponse> GetByIdAsync(int id);
    Task<ReviewResponse> AddReviewAsync(int productId, string reviewerName, CreateReviewRequest request);
    Task<IEnumerable<ProductResponse>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductResponse>> GetByBrandAsync(int brandId);
    Task<IEnumerable<ProductResponse>> SearchAsync(string searchTerm);
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    Task<IEnumerable<BrandResponse>> GetAllBrandsAsync();
}
