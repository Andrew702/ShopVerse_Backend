using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceAPI.Models
{
    public class orderItems
    {
        public string id { get; set; }

        public int quantity { get; set; }

        [ForeignKey("Order")]
        public string orderId { get; set; }

        [JsonIgnore]
        public virtual Order? Order { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }


    }
}
