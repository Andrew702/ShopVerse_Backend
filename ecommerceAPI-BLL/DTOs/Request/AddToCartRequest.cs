namespace ecommerceAPI.BLL.DTOs.Request;

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}
