using ecommerceAPI.DAL.Enums;

namespace ecommerceAPI.DAL.Entities;

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Foreign Key
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
