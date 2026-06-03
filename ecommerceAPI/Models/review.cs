using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerceAPI.Models
{
    public class review
    {
        [Key]
        public int Id { get; set; }

        public decimal rating { get; set; }

        public string comment { get; set; }

        public DateTime date {  get; set; }

        public string reviewerName { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [JsonIgnore]
        public virtual Product? Product { get; set; }
    }
}