namespace ecommerceAPI.BLL.DTOs.Response;

public class WishlistResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public string ProductImage { get; set; } = string.Empty;
}
