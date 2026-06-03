using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceAPI.Models
{
    public class cartItems
    {
        public string id { get; set; }

        public int quantity { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [JsonIgnore]

        public virtual Product? Product { get; set; }
    }
}
