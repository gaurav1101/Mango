using Mango.ServiceBus;
using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public CartController(IShoppingCartService shoppingCartService,IOrderService orderService)
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        public async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            try
            {
                var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
                //var email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
                //var name = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Name)?.FirstOrDefault()?.Value;
                //var phone
                var responseDto = await _shoppingCartService.GetCartByUserIdAsync(user);
                if (responseDto.Result != null && responseDto.Error==null)
                {
                    CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                    TempData["Success"] = "Cart Loaded";
                    return response;
                }
                else
                {
                    CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
                    TempData["Error"] = responseDto.Error;
                    return response;
                }
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
            }
           
            return new CartDto();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
           var detailsTosend = await _shoppingCartService.GetCartByUserIdAsync(user);
            var data = JsonConvert.DeserializeObject<CartDto>(detailsTosend.Result.ToString());
            data.CartHeaderDto.CouponCode = cartDto.CartHeaderDto.CouponCode;
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

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(CartDto cartDto)
        {
            try
            {
                CartDto res = await LoadCartDtoBasedOnLoggedInUser();
                res.CartHeaderDto.phone = cartDto.CartHeaderDto.phone;
                res.CartHeaderDto.Name = cartDto.CartHeaderDto.Name;
                res.CartHeaderDto.Email = cartDto.CartHeaderDto.Email;
                var response = await _orderService.CreateOrderAsync(res);
                if (response.Result != null)
                {
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    OrderHeaderDto cart = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                    var stripeRequest = new StripeRequestDto
                    {
                        orderHeaderDto = cart,
                        ApprovedUrl = domain + "Cart/OrderConfirmation/orderId=" + cart.OrederHeaderId,
                        CancelUrl = domain + "Cart/CheckOut"
                    };
                    var stripeSession = await _orderService.CreateStripeSessionAsync(stripeRequest);
                    StripeRequestDto stripeRequestDto = JsonConvert.DeserializeObject<StripeRequestDto>(stripeSession.Result.ToString());
                    Response.Headers.Add("Location", stripeRequestDto.StripeSessionUrl);
                    
                }
                return new StatusCodeResult(303);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(400);
            }
           
        }

        [HttpGet("Cart/OrderConfirmation/orderId={orderId}")]
		public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var response =await _orderService.ValidateStripeSession(orderId);
            if(response.Result != null)
            {
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(response.Result.ToString());
                if (orderHeaderDto.Status == SD.Approved)
                {
                    return View(orderId);
                }
            }
            return View(orderId);
        }
    }
}


//When User will click on addToCart button
//Send product ID from the button click.
// call Shopping cart API by writing a methid in cartController.