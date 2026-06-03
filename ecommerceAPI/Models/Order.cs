using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceAPI.Models
{
    public class Order
    {
        public string id { get; set; }

        public decimal total { get; set; }

        public DateTime date { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}