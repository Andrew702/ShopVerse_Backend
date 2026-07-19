namespace ecommerceAPI.BLL.DTOs.Response;

public class ProductResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public class ProductDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
    public CategoryResponse Category { get; set; } = null!;
    public BrandResponse Brand { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ReviewResponse> Reviews { get; set; } = new();
}
