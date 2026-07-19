using ecommerceAPI.BLL.DTOs.Response;

namespace ecommerceAPI.BLL.Interfaces;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(int page, int pageSize, string? search, int? categoryId, int? brandId);
    Task<ProductDetailResponse?> GetByIdAsync(int id);
    Task<IEnumerable<ProductResponse>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductResponse>> GetByBrandAsync(int brandId);
    Task<IEnumerable<ProductResponse>> SearchAsync(string searchTerm);
    Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    Task<IEnumerable<BrandResponse>> GetAllBrandsAsync();
}
