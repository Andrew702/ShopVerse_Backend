using ecommerceAPI.Models;

namespace ecommerceAPI.DTO
{
    public class WishlistDTO
    {
        //            {
        //                "id": "1",
        //    "name": "Ali",
        //    "email": "ali@test.com",
        //    "phone": "0111111111",
        //    "password": "123456",
        //    "wishlist": [
        //        6,
        //        13
        //    ],
        //    "cart": [
        //        {
        //                    "id": "8eaa86db-053b-4aa7-8724-627850bb0227",
        //            "productId": "2",
        //            "quantity": 1
        //        },
        //        {
        //                    "id": "fcf36f17-340f-455b-bf80-779cd7b9153a",
        //            "productId": "1",
        //            "quantity": 1
        //        },
        //        {
        //                    "id": "803b4ade-eec8-447b-904f-633f3cf64509",
        //            "productId": "6",
        //            "quantity": 1
        //        }
        //    ],
        //    "orders": []
        //}
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public string phone { get; set; }

        public string password { get; set; }

        public ICollection<Order> orders { get; set; }

        public ICollection<cartItems> cart { get; set; }

        public ICollection<int> wishlist { get; set; }
    }
}
