using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDto> GetCouponByIdAsync(int couponId);
        Task<ResponseDto> GetCouponAsync(string couponCode);
        Task<ResponseDto> GetAllCouponsAsync();
        Task<ResponseDto> AddCouponAsync(CouponDto couponDto);
        Task<ResponseDto> UpdateCouponAsync(int couponId, CouponDto couponDto);
        Task<ResponseDto> DeleteCouponAsync(int couponId);
		Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
	}
}
