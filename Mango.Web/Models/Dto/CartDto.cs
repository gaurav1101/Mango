using Mango.Mango.Web.Models;

namespace Mango.Web.Models
{
    public class CartDto
    {
        public CartHeaderDto CartHeaderDto { get; set; }
        public IEnumerable<CartDetailsDto> CartDetailsDtos { get; set; }
        public CouponDto? coupondto { get; set; }
    }
}
