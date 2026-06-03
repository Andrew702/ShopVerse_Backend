using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerceAPI.Models
{
    public class orderItems
    {
        public string id { get; set; }

        public int quantity { get; set; }

        [ForeignKey("Order")]
        public string orderId { get; set; }

        public virtual Order? Order { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product? Product { get; set; }


    }
}
