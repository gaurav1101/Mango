using Mango.ServiceBus;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
       

        public CartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
            
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        public async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var responseDto = await _shoppingCartService.GetCartByUserIdAsync(user);
            if (responseDto.Result != null)
            {
                CartDto response=JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Item added to cart";
                return response;
            }
            return new CartDto();
        }

        [HttpPost]
        public async Task<CartDto> ApplyCoupon(CartDto cartDto)
        {
             cartDto = new CartDto
            {
                CartHeaderDto = new Mango.Web.Models.CartHeaderDto
                {
                    CouponCode = cartDto.CartHeaderDto.CouponCode,
                    UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value,
                     CartHeaderId=cartDto.CartHeaderDto.CartHeaderId
                     
                }
            };
            var responseDto = await _shoppingCartService.ApplyCouponAsync(cartDto);
            if (responseDto != null)
            {
                CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Item added to cart";
                return response;
            }
            return new CartDto();
        }

        //[HttpPost]
        [Authorize]
        public async Task<CartDto> EmailCartRequest(CartDto cartDto)
        {
            var responseDto=_shoppingCartService.EmailCartAsync(cartDto);
            if(responseDto != null)
            {
                CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Email sent ";
                return response;
            }
            TempData["Error"] = "Email not sent ";
            return new CartDto();
        }

        public async Task<IActionResult> Remove(int carDetailsId)
        {
            var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var responseDto = await _shoppingCartService.RemoveFromCartAsync(carDetailsId);
            if (responseDto != null)
            {
                CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Item Removed from your cart";
                return RedirectToAction("CartIndex");
            }
            TempData["Error"] = "Something went wrong item can`t be removed.";
            return View(); 
        }
    }
}


//When User will click on addToCart button
//Send product ID from the button click.
// call Shopping cart API by writing a methid in cartController.