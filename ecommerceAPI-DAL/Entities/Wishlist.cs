namespace ecommerceAPI.DAL.Entities;

public class Wishlist
{
    public int Id { get; set; }

    // Foreign Keys
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
