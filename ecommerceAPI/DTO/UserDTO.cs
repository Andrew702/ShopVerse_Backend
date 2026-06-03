using ecommerceAPI.Models;

namespace ecommerceAPI.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public ICollection<Order> orders { get; set; }

        public ICollection<cartItems> cartItems { get; set; }

        public ICollection<Wishlist> wishlists { get; set; }
    }
}
