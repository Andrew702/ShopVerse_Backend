using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ecommerceAPI.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public decimal price { get; set; }

        public string category { get; set; }

        public string image { get; set; }

        public virtual ICollection<review> reviews { get; set; }
    }
}
