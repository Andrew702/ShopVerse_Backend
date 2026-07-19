namespace ecommerceAPI.DAL.Entities;

public class Review
{
    public int Id { get; set; }
    public decimal Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string ReviewerName { get; set; } = string.Empty;

    // Foreign Key
    public int ProductId { get; set; }

    // Navigation
    public virtual Product Product { get; set; } = null!;
}
