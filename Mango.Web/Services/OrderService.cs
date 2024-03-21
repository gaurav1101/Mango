using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using System;

namespace Mango.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {

            _baseService = baseService;

        }

        public async Task<ResponseDto> CreateOrderAsync(CartDto cartDto)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = cartDto,
                url = SD.OrderAPIBaseUrl + "/api/OrderAPI/CreateOrder",
            });
        }

		public async Task<ResponseDto> GetOrdersAsync(string? userId)
		{
          
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.GET,
				//Data = cartDto,
                
                url = SD.OrderAPIBaseUrl + "/api/OrderAPI/GetOrders?userId=" + userId,
			});
		}

		public async Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto )
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = stripeRequestDto,
                url = SD.OrderAPIBaseUrl + "/api/OrderAPI/CreateStripeSession",
            });
        }

        public async Task<ResponseDto> ValidateStripeSession(int orderId)
        {
            return await _baseService.sendAsync(new Models.Dto.RequestDto()
            {
                ApiType = SD.ApiTypes.POST,
                Data = orderId,
                url = SD.OrderAPIBaseUrl + "/api/OrderAPI/ValidateStripeSession",
            });
        }

		public async Task<ResponseDto> GetOrderByIdAsync(int orderId)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.GET,
				//Data = cartDto,
				url = SD.OrderAPIBaseUrl + "/api/OrderAPI/GetOrderById/"+orderId,
			});
		}

		public async Task<ResponseDto> UpdateOrderStatusAsync(int orderId, string newStatus)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.POST,
				Data = newStatus,
				url = SD.OrderAPIBaseUrl + "/api/OrderAPI/UpdateOrderStatus/"+orderId,
			});
		}
	}
}
