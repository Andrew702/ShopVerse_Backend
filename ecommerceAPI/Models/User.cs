using Microsoft.AspNetCore.Identity;

namespace ecommerceAPI.Models
{
    public class User : IdentityUser
    {
        public virtual ICollection<Order>? orders { get; set; }

        public virtual ICollection<cartItems>? CartItems { get; set; }

        public virtual ICollection<Wishlist>? Wishlists { get; set; }

    }
}
