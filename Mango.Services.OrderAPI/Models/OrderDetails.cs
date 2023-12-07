using Mango.Services.OrderAPI.Models.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public int OrderHeaderId { get; set; }
        public OrderHeader? CartHeader { get; set; }
        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Products { get; set; }
        public int Count { get; set; }
        public string? ProductName { get; set; }
        public string? Price { get; set; }
    }
}
