namespace ecommerceAPI.Models
{
    public class Order
    {
        public string id { get; set; }

        public decimal total { get; set; }

        public DateTime date { get; set; }
    }
}