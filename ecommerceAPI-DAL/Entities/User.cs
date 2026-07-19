using Microsoft.AspNetCore.Identity;

namespace ecommerceAPI.DAL.Entities;

public class User : IdentityUser
{
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
