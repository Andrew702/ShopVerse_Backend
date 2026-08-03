namespace ecommerceAPI.BLL.DTOs.Response;

public class ProductResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string Image { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int BrandId { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsOnSale { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
}

public class ProductDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string Image { get; set; } = string.Empty;
    public CategoryResponse Category { get; set; } = null!;
    public BrandResponse Brand { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public bool IsOnSale { get; set; }
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public List<ReviewResponse> Reviews { get; set; } = new();
}
