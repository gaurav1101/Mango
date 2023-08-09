using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService,IConfiguration configuration)
        {
            _baseService = baseService;

        }

        public async Task<ResponseDto> GetCouponByIdAsync(int couponId)
        {
             return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
               ApiType=SD.ApiTypes.GET,
               Data = couponId,
               url= SD.CouponAPIBaseUrl+ "/api/CouponAPI/" + couponId,
               token=""
            });
        }

        public async Task<ResponseDto> GetCouponAsync(string couponCode)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.GET,
                Data = couponCode,
                url = SD.CouponAPIBaseUrl + "/api/CouponAPI",
                token = ""
            });
        }

        public async Task<ResponseDto> GetAllCouponsAsync()
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.GET,
                url = SD.CouponAPIBaseUrl + "/api/CouponAPI",
                token = ""
			});
        }

        public async Task<ResponseDto> AddCouponAsync(CouponDto couponDto)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = couponDto,
                url = SD.CouponAPIBaseUrl + "/api/CouponAPI",
                token = ""
            });
        }

        public async Task<ResponseDto> UpdateCouponAsync(int couponId, CouponDto couponDto)
        {
            return await  _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.PUT,
                Data = couponDto,
                url = SD.CouponAPIBaseUrl + "/api/CouponAPI/"
                + couponId,
                token = ""
            });
        }

        public async Task<ResponseDto> DeleteCouponAsync(int couponId)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.DELETE,
                url = SD.CouponAPIBaseUrl + "/api/CouponAPI/"
                + couponId,
                token = ""
            });
        }
    }
}
