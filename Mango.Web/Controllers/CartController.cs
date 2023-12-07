using Mango.ServiceBus;
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
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
            //var email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            //var name = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Name)?.FirstOrDefault()?.Value;
            //var phone
            var responseDto = await _shoppingCartService.GetCartByUserIdAsync(user);
            if (responseDto.Result != null)
            {
                CartDto response=JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Cart Loaded";
                return response;
            }
            return new CartDto();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string coupon)
        {
            var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
           var detailsTosend = await _shoppingCartService.GetCartByUserIdAsync(user);
            var data = JsonConvert.DeserializeObject<CartDto>(detailsTosend.Result.ToString());
            var responseDto = await _shoppingCartService.ApplyCouponAsync(data);
            if (responseDto != null)
            {
                CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Discount Coupon Applied";
                return View("CartIndex", data);
            }
            return View("CartIndex", data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EmailCartRequest(CartDto cartDto)
        {
            try
            {
                var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                var userEmail=User.Claims.Where(u=>u.Type== JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
                var detailsTosend = await _shoppingCartService.GetCartByUserIdAsync(user);
                var data=JsonConvert.DeserializeObject<CartDto>(detailsTosend.Result.ToString());
                data.CartHeaderDto.Email = userEmail;
                var responseDto =await _shoppingCartService.EmailCartRequest(data);
                if (responseDto.Result != null)
                {
                    CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                    TempData["Success"] = "Email sent ";
                    return View("CartIndex", data);
                }
                TempData["Error"] = "Email not sent ";
                return View("CartIndex", data);
            }
           catch (Exception ex)
            {
                var exc = ex.Message;
                TempData["Error"] = exc;
                return  View(nameof(CartIndex));
            }
        }

        public async Task<IActionResult> Remove(int carDetailsId)
        {
            var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var responseDto = await _shoppingCartService.RemoveFromCartAsync(carDetailsId);
            if (responseDto.Result != null)
            {
                CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                TempData["Success"] = "Item Removed from your cart";
                return RedirectToAction("CartIndex");
            }
            TempData["Error"] = "Something went wrong item can`t be removed.";
            return View(); 
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }
    }
}


//When User will click on addToCart button
//Send product ID from the button click.
// call Shopping cart API by writing a methid in cartController.