using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
	
	public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
		private readonly ITokenProvider _tokenProvider;
        private readonly IShoppingCartService _shoppingCartService;
		ResponseDto _response;
        public CouponController(ICouponService couponService,IShoppingCartService shoppingCartService)
        {
            _couponService = couponService;
            _shoppingCartService= shoppingCartService;
             _response = new ResponseDto();
            
        }

        [HttpGet]
		
		public async Task<IActionResult> CouponIndex()
        {
            try
            {
                
				IEnumerable<CouponDto> couponDtos;
                var coupons = await _couponService.GetAllCouponsAsync();
                if (coupons.IsSuccess && coupons.Result != null)
                {
                    couponDtos = JsonConvert.DeserializeObject<List<CouponDto>>(coupons.Result.ToString());
                    return View(couponDtos);
                }
                TempData["error"] = coupons.Error.ToString();
                couponDtos = new List<CouponDto>();
				return View(couponDtos);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ApplyCoupon(string couponCode)
		{
			var user = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var coupons = await _couponService.GetCouponAsync(couponCode);
			CouponDto coupon = JsonConvert.DeserializeObject<CouponDto>(coupons.Result.ToString());
			var detailsTosend = await _shoppingCartService.GetCartByUserIdAsync(user);
			var data = JsonConvert.DeserializeObject<CartDto>(detailsTosend.Result.ToString());
            data.CartHeaderDto.CouponCode = coupon.CouponCode;
			var responseDto = await _shoppingCartService.ApplyCouponAsync(data);
			if (responseDto != null)
			{
				CartDto response = JsonConvert.DeserializeObject<CartDto>(responseDto.Result.ToString());
				TempData["Success"] = "Discount Coupon Applied";
                
				return RedirectToPage("Cart/CartIndex", data);
			}
			return RedirectToPage("CartIndex", data);
		}

		//[HttpGet]

		//private async Task<IActionResult> GetEligibleCouponForUser(int total)
		//{
		//	try
		//	{

		//		IEnumerable<CouponDto> couponDtos;
		//		var coupons = await _couponService.GetAllCouponsAsync();
		//		if (coupons.IsSuccess && coupons.Result != null)
		//		{
		//			couponDtos = JsonConvert.DeserializeObject<List<CouponDto>>(coupons.Result.ToString());
		//                  foreach(var data in couponDtos)
		//                  {
		//                      if(data.MinimumAmount>)
		//                  }
		//			return View(couponDtos);
		//		}
		//		TempData["error"] = coupons.Error.ToString();
		//		couponDtos = new List<CouponDto>();
		//		return View(couponDtos);
		//	}
		//	catch (Exception ex)
		//	{
		//		return View(ex);
		//	}
		//}

		//[Authorize]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        [HttpPost]
       
        public async Task<IActionResult> CreateCoupon(CouponDto couponDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var coupons = await _couponService.AddCouponAsync(couponDto);
                    if (coupons.IsSuccess && coupons.Result != null)
                    {
                        TempData["success"] = "Created Successfully";
                        return RedirectToAction(nameof(CouponIndex));
                    }
                }
                else
                {
                    TempData["error"] = _response.Error;
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [Authorize]
        public async Task<IActionResult> RemoveCoupon(int couponId)
		{
            try
            {
                if (couponId > 0)
                {
                    var couponToBeDeleted = await _couponService.GetCouponByIdAsync(couponId);
                    if(couponToBeDeleted != null)
                    {
                       var coupon= JsonConvert.DeserializeObject<CouponDto>(couponToBeDeleted.Result.ToString());

                        return View(coupon);
                    }
                    else
                    {
                        TempData["error"] = _response.Error;
                    }
                }
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RemoveCoupon(CouponDto couponDto)
        {
            try
            {
                    var coupons = await _couponService.DeleteCouponAsync(couponDto.CouponId);
                    if (coupons.IsSuccess && coupons.Result != null)
                    {
                    TempData["success"] ="Deleted Successfully";
                    return RedirectToAction(nameof(CouponIndex));
                    }
                else
                {
                    TempData["error"] = _response.Error;
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
