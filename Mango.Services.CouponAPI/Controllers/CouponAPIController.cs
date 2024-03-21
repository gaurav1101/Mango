using AutoMapper;
using Azure;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using Stripe;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dBContext;
        private ResponseDto _response;
        private readonly IMapper _mapper;
        //private  readonly ILogger _logger;
        public CouponAPIController(ApplicationDBContext dBContext, IMapper mapper) 
        { 
            _response= new ResponseDto();
            _dBContext = dBContext;
            _mapper = mapper;
        }
        // GET: api/<CouponAPIController>
        [HttpGet]
        public ActionResult<ResponseDto> Get()
        {
            try
            {
              _response.Result=  _mapper.Map<IEnumerable<CouponDto>>(_dBContext.Coupons.ToList());
               return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest.ToString();
                _response.Error =  ex.Message.ToString();
                return _response;
            }
        }

        // GET api/<CouponAPIController>/
        [HttpGet("{id}")]
        public ActionResult<ResponseDto> Get(int id)
        {
             var coupon= _dBContext.Coupons.FirstOrDefault(u=>u.CouponId==id);
            if(coupon==null)
            {
                _response.IsSuccess = false;
                _response.Error = "No Such coupon exists";
            }
            _response.Result= _mapper.Map<CouponDto>(coupon);
            return _response;
        }


        // GET api/<CouponAPIController>/
        [HttpGet]
        [Route("GetByCode/{CouponCode}")]
        public ActionResult<ResponseDto> GetByCode(string CouponCode)
        {
            try
            {
                var coupon = _dBContext.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == CouponCode.ToLower());
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Error = "No Such Coupon exists";
                }
                _response.Result = _mapper.Map<CouponDto>(coupon);
                return _response;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error= ex.Message.ToString();
                return _response;
            }
        }

        // POST api/<CouponAPIController>
        [HttpPost]
        //[Authorize(AuthenticationSchemes = "Bearer" ,Roles ="Admin")]
        public ResponseDto Post(CouponDto couponDto)
        {
            try
            {
                if (couponDto == null)
                {

                    _response.IsSuccess = false;
                    _response.Error = "Enter some valid values";
                    return _response;
                }
                StripeConfiguration.ApiKey = "sk_test_51OM28kSFrpBxnPQ23HGmoTOrpqMsUt27DULatAA3Ut72V1Y92j9hDeDcZQ39gLVeAS7Gqu6gVWiUzQ7kBM2pg4GC00cOCUEwpQ";

                var options = new CouponCreateOptions
                {
                    AmountOff=(long)couponDto.DiscountAmount*100,
                    Id= couponDto.CouponCode,
                    Name = couponDto.CouponCode,
                    Currency="usd",
                };
                var service = new CouponService();
                service.Create(options);
                _dBContext.Coupons.Add(_mapper.Map<Models.Coupon>(couponDto));
                _dBContext.SaveChanges();
                _response.Result = couponDto;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error=ex.Message.ToString();
                return _response;
            }
        }

        // PUT api/<CouponAPIController>/5
        [HttpPut("{id}")]
        public ResponseDto Put(int id, [FromBody] CouponDto couponDto)
        {
            try
            {
                if (id < 0 && couponDto.CouponId != id)
                {
                    _response.IsSuccess = false;
                    _response.Error = "Please enter valid data";
                    return _response;
                }
                _dBContext.Coupons.Update(_mapper.Map<Models.Coupon>(couponDto));
                _dBContext.SaveChanges();
                _response.Result= couponDto;
                return _response;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        // DELETE api/<CouponAPIController>/5
        [HttpDelete("{id}")]
		//[Authorize("ADMIN")]
		public ResponseDto Delete(int id)
        {
            try
            {
                if (id <= 0 )
                {
                    _response.IsSuccess = false;
                    _response.Error = "Please enter valid Id";
                    return _response;
                }
                var data=_dBContext.Coupons.FirstOrDefault(u=>u.CouponId==id);
                if (data!=null)
                {
                    _dBContext.Coupons.Remove(data);
                    _dBContext.SaveChanges();
                    var service = new CouponService();
                    service.Delete(data.CouponCode);
                    _response.Result = _mapper.Map<CouponDto>(data );
                }
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }
    }
}
