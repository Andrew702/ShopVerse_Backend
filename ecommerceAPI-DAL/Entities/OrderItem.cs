namespace ecommerceAPI.DAL.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Foreign Keys
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    // Navigation
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
