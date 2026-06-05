using ecommerceAPI.Models;

namespace ecommerceAPI.DTO
{
    public class CartItemDTO
    {
        //      [{
        //      id: item.id,
        //  productId: item.productId,
        //  quantity: item.quantity,
        //  product: item.product,
        //}
        //      ;]

        public string id { get; set; }

        public int productId { get; set; }

        public int quantity { get; set; }

    }
}
