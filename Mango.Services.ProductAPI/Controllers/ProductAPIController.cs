using AutoMapper;
using Azure;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection.Metadata.Ecma335;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dBContext;
        private ResponseDto _response;
        private readonly IMapper _mapper;
        public ProductAPIController(ApplicationDBContext dBContext, IMapper mapper) 
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
              _response.Result=  _mapper.Map<IEnumerable<ProductDto>>(_dBContext.products.ToList());
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
             var coupon= _dBContext.products.FirstOrDefault(u=>u.ProductId ==id);
            if(coupon==null)
            {
                _response.IsSuccess = false;
                _response.Error = "No Such coupon exists";
            }
            _response.Result= _mapper.Map<ProductDto>(coupon);
            return _response;
        }

        // POST api/<CouponAPIController>
        [HttpPost]
        //[Authorize("ADMIN")]
        public ResponseDto Post(ProductDto productDto)
        {
            try
            {
                if (productDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Error = "Enter some valid values";
                    return _response;
                }

                _dBContext.products.Add(_mapper.Map<Product>(productDto));
                _dBContext.SaveChanges();
                _response.Result = productDto;
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
        public ResponseDto Put(int id, [FromBody] ProductDto productDto)
        {
            try
            {
                if (id < 0 && productDto.ProductId != id)
                {
                    _response.IsSuccess = false;
                    _response.Error = "Please enter valid data";
                    return _response;
                }
                _dBContext.products.Update(_mapper.Map<Product>(productDto));
                _dBContext.SaveChanges();
                _response.Result= productDto;
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
                var data=_dBContext.products.FirstOrDefault(u=>u.ProductId==id);
                if (data!=null)
                {
                    _dBContext.products.Remove(data);
                    _dBContext.SaveChanges();
                    _response.Result = _mapper.Map<ProductDto>(data );
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
