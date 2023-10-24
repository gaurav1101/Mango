using Mango.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Services.IService
{
    public interface IShoppingCartService
    {
          Task<ResponseDto> GetCartByUserIdAsync(string userId);
          Task<ResponseDto> ApplyCouponAsync(CartDto cartDto);
          Task<ResponseDto> UpsertCartAsync(CartDto cartDto);
          Task<ResponseDto> RemoveFromCartAsync(int cartDetailsId);
          Task<ResponseDto> EmailCartRequest(CartDto cartDto);
    }
}
