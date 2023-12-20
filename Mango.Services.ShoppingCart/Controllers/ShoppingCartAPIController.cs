using AutoMapper;
using Mango.ServiceBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dBContext;
        private ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public ShoppingCartAPIController(ApplicationDBContext dBContext, IConfiguration configuration, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus)
        {
            _response = new ResponseDto();
            _dBContext = dBContext;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            CartHeaderDto cartHeaderDto = _mapper.Map<CartHeaderDto>(_dBContext.CartHeaders.FirstOrDefault(u => u.UserId == userId));
            List<CartDetailsDto> cartDetailsDto = _mapper.Map<List<CartDetailsDto>>(_dBContext.CartDetails.Where(u => u.CartHeaderId == cartHeaderDto.CartHeaderId));
            CartDto cartDto = new()
            {
                CartHeaderDto = cartHeaderDto,
                CartDetailsDtos = cartDetailsDto
            };
          //  var user = _dBContext.CartHeaders.Include(u => u.UserId).ToList();
            IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

            foreach (var item in cartDetailsDto)
            {
                item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                if (item.Product == null)
                {
                    cartDto.CartHeaderDto.CartTotal = 90;
                }
                else
                {
                    cartDto.CartHeaderDto.CartTotal += (item.Count * item.Product.Price);
                }
            }

            //apply coupon if any
            if (!string.IsNullOrEmpty(cartDto.CartHeaderDto.CouponCode))
            {
                CouponDto couponDto = await _couponService.GetCoupon(cartDto.CartHeaderDto.CouponCode);
                if (couponDto != null && cartDto.CartHeaderDto.CartTotal > couponDto.MinimumAmount)
                {
                    cartDto.CartHeaderDto.CartTotal -= couponDto.MinimumAmount;
                    cartDto.CartHeaderDto.Discount = couponDto.DiscountAmount;
                    _response.Result = cartDto;
                }
                else
                {
                    _response.Result = cartDto;
                    _response.Error = "Coupon cannot be applied Min order value must be atleast " + couponDto.MinimumAmount;
                }
               
            }
            else
            {
                _response.Result = cartDto;
            }
            
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto, int id)
        {
            var cartHeaderFromDb = await _dBContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeaderDto.UserId);
            cartHeaderFromDb.CouponCode = cartDto.CartHeaderDto.CouponCode;
            _dBContext.CartHeaders.Update(cartHeaderFromDb);
            await _dBContext.SaveChangesAsync();
            _response.Result = cartDto;
            return _response;
        }


        [HttpPost("EmailCartRequest")]
        // [Authorize]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            CartHeaderDto cartHeaderDto = _mapper.Map<CartHeaderDto>(_dBContext.CartHeaders.FirstOrDefault(u => u.UserId == cartDto.CartHeaderDto.UserId));
            //cartDto.CartHeaderDto.Email = cartHeaderDto.Email;
            await _messageBus.publishMessage(cartDto, _configuration.GetValue<string>("ServiceBusConfig:ServiecBusName"));
            _response.Result = new CartDto();
            _response.IsSuccess = true;
            return _response;
        }


        [HttpPost("RemoveCoupon")]
        public async Task<ResponseDto> RemoveCoupon(int userId)
        {
            var cartHeaderFromDb = await _dBContext.CartHeaders.FirstAsync(u => u.UserId == userId.ToString());
            cartHeaderFromDb.CouponCode = "";
            _dBContext.CartHeaders.Update(cartHeaderFromDb);
            await _dBContext.SaveChangesAsync();
            _response.Result = true;
            return _response;
        }


        [HttpPost]
        //[Authorize("ADMIN")]
        public async Task<ResponseDto> Upsert(CartDto cartDto)
        {
            try
            {
                CartDetails cartDetails = new CartDetails();
                if (cartDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Error = "Enter some valid values";
                    return _response;
                }

                //check if header is alreay present or not
                // if not then add header info and headerId in cartDetals

                var cartheaderfromDB = await _dBContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeaderDto.UserId);
                if (cartheaderfromDB == null)
                {
                    var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeaderDto);
                    _dBContext.CartHeaders.Add(cartHeader);
                    await _dBContext.SaveChangesAsync();

                    cartDto.CartDetailsDtos.First().CartHeaderId = cartHeader.CartHeaderId;
                    _dBContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
                    await _dBContext.SaveChangesAsync();

                }
                //if header is not null 
                // check if the details has the same product
                else
                {
                    var cartDetailsFromDb = await _dBContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetailsDtos.First().ProductId
                                             && u.CartHeaderId == cartheaderfromDB.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        cartDto.CartDetailsDtos.First().CartHeaderId = cartheaderfromDB.CartHeaderId;
                        _dBContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
                        await _dBContext.SaveChangesAsync();
                    }

                    else
                    //update count in cart details
                    {
                        cartDetailsFromDb.Count += cartDto.CartDetailsDtos.First().Count;
                        //cartDto.CartDetailsDtos.First().CartHeaderId += cartDetailsFromDb.CartHeaderId;
                        //cartDto.CartDetailsDtos.First().CartDetailsId += cartDetailsFromDb.CartDetailsId;
                        _dBContext.CartDetails.Update(cartDetailsFromDb);
                        await _dBContext.SaveChangesAsync();
                    }

                }
                _response.Result = cartDto;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error = ex.Message.ToString();
                return _response;
            }
        }

        [HttpDelete("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            var cartDetail = _dBContext.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
            int cartDetailsCount = _dBContext.CartDetails.Where(u => u.CartHeaderId == cartDetail.CartHeaderId).Count();
            //
            if (cartDetailsCount == 1)
            {
                var cartHeader = _dBContext.CartHeaders.First(u => u.CartHeaderId == cartDetail.CartHeaderId);
                _dBContext.CartHeaders.Remove(cartHeader);
                await _dBContext.SaveChangesAsync();
                _response.IsSuccess = true;
                _response.Result = cartDetail;
                return _response;
            }
            _dBContext.Remove(cartDetail);
            await _dBContext.SaveChangesAsync();
            _response.IsSuccess = true;
            _response.Result = cartDetail;
            return _response;
        }

        //public async Task<ResponseDto> getUser([FromBody] int cartDetailsId)
        //{
        //}
    }
}
