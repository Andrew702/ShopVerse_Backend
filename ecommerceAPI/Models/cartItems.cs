using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerceAPI.Models
{
    public class cartItems
    {
        public string id { get; set; }

        public int quantity { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual User? User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
