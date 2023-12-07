using Mango.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderAPI.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrederHeaderId { get; set; }
        public string? UserId { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? phone { get; set; }
        //public string status { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? StripeSessionId { get; set; }

        public IEnumerable<OrderDetails> orderDetails { get; set; }
    }
}
