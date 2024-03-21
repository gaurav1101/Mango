using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IService
{
    public interface IOrderService
    {
       // Task<ResponseDto> GetOrderAsync(string userId);
        Task<ResponseDto> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto> CreateStripeSessionAsync(StripeRequestDto stripeRequestDto);
        Task<ResponseDto> ValidateStripeSession(int orderId);
        Task<ResponseDto> GetOrdersAsync(string userId);
		Task<ResponseDto> GetOrderByIdAsync(int orderId); 
        Task<ResponseDto> UpdateOrderStatusAsync(int orderId, string newStatus); 
        //Task<ResponseDto> GetOrdersAsync();
	}
}
