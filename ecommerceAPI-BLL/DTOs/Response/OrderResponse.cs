namespace ecommerceAPI.BLL.DTOs.Response;

public class OrderResponse
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<OrderItemResponse> OrderItems { get; set; } = new();
}

public class OrderItemResponse
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = string.Empty;
    public string ProductImage { get; set; } = string.Empty;
}
