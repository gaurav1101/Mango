using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        ResponseDto _response;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
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
                else
                {
                    TempData["error"] = _response.Error;
                }
                return View(coupons);
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

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
