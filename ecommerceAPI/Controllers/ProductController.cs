using System.Security.Claims;
using ecommerceAPI.BLL.DTOs.Request;
using ecommerceAPI.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? brandId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] double? minRating = null,
        [FromQuery] bool? onSale = null,
        [FromQuery] bool? inStock = null,
        [FromQuery] string? sortBy = null)
    {
        var result = await _productService.GetAllAsync(
            page, pageSize, search, categoryId, brandId,
            minPrice, maxPrice, minRating, onSale, inStock, sortBy);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost("{id:int}/reviews")]
    [Authorize]
    public async Task<IActionResult> AddReview(int id, [FromBody] CreateReviewRequest request)
    {
        var reviewerName = User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
        var review = await _productService.AddReviewAsync(id, reviewerName, request);
        return Ok(review);
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var products = await _productService.GetByCategoryAsync(categoryId);
        return Ok(products);
    }

    [HttpGet("brand/{brandId:int}")]
    public async Task<IActionResult> GetByBrand(int brandId)
    {
        var products = await _productService.GetByBrandAsync(brandId);
        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search term is required." });
        var products = await _productService.SearchAsync(q);
        return Ok(products);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _productService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _productService.GetAllBrandsAsync();
        return Ok(brands);
    }
}
