namespace ecommerceAPI.BLL.DTOs.Response;

public class ReviewResponse
{
    public int Id { get; set; }
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
}
