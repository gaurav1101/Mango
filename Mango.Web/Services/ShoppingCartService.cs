using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IBaseService _baseService;
        public ShoppingCartService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;

        }
        public async Task<ResponseDto> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,     
                Data = cartDto,
                url = SD.ShoppingCartAPIBaseUrl + "/api/ShoppingCartAPI/ApplyCoupon",
            });
        }

        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.GET,
                Data = "",
                url = SD.ShoppingCartAPIBaseUrl + "/api/ShoppingCartAPI/GetCart/" + userId,
            }); 
        }

        public async Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.DELETE,
                Data = "",
                url = SD.ShoppingCartAPIBaseUrl + "/api/ShoppingCartAPI/" + cartDetailsId,
            });
        }

        public async Task<ResponseDto> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = cartDto,
                url = SD.ShoppingCartAPIBaseUrl + "/api/ShoppingCartAPI",
            });
        }

        public async Task<ResponseDto> EmailCartAsync(CartDto cartDto)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = cartDto,
                url = SD.ShoppingCartAPIBaseUrl + "/api/ShoppingCartAPI/EmailCart",//https://localhost:7003/api/ShoppingCartAPI/EmailCart
            });
        }
    }
}
