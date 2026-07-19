namespace ecommerceAPI.BLL.DTOs.Request;

public class CreateReviewRequest
{
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
