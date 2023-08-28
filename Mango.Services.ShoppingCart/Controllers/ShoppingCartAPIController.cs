using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
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
   // [Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly ApplicationDBContext _dBContext;
        private ResponseDto _response;
        private readonly IMapper _mapper;
        public ShoppingCartAPIController(ApplicationDBContext dBContext, IMapper mapper) 
        { 
            _response= new ResponseDto();
            _dBContext = dBContext;
            _mapper = mapper;
        }
       
        // POST api/<CouponAPIController>
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
                if(cartheaderfromDB == null)
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
                    var cartDetailsFromDb=await _dBContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(u=>u.ProductId==cartDto.CartDetailsDtos.First().ProductId
                                             && u.CartHeaderId==cartDto.CartHeaderDto.CartHeaderId);
                    if(cartDetailsFromDb == null)
                    {
                        cartDto.CartDetailsDtos.First().CartHeaderId = cartDto.CartHeaderDto.CartHeaderId;
                        _dBContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
                        await _dBContext.SaveChangesAsync();
                    }
                    
                    else
                    //update count in cart details
                    {
                        cartDto.CartDetailsDtos.First().Count += cartDetailsFromDb.Count;
                        //cartDto.CartDetailsDtos.First().CartHeaderId += cartDetailsFromDb.CartHeaderId;
                        //cartDto.CartDetailsDtos.First().CartDetailsId += cartDetailsFromDb.CartDetailsId;
                        _dBContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
                        await _dBContext.SaveChangesAsync();
                    }
                   
                }
                _response.Result = cartDto;
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Error=ex.Message.ToString();
                return _response;
            }
        }

        //[HttpDelete]
        ////[Authorize("ADMIN")]
        //public async Task<ResponseDto> RemoveCart(CartDto cartDto)
        //{
        //    try
        //    {
        //        CartDetails cartDetails = new CartDetails();
        //        if (cartDto == null)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Error = "Enter some valid values";
        //            return _response;
        //        }

        //        //check if header is alreay present or not
        //        // if not then add header info and headerId in cartDetals

        //        var cartheaderfromDB = await _dBContext.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeaderDto.UserId);
        //        if (cartheaderfromDB == null)
        //        {
        //            var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeaderDto);
        //            _dBContext.CartHeaders.Add(cartHeader);
        //            await _dBContext.SaveChangesAsync();

        //            cartDto.CartDetailsDtos.First().CartHeaderId = cartHeader.CartHeaderId;
        //            _dBContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
        //            await _dBContext.SaveChangesAsync();

        //        }
        //        //if header is not null 
        //        // check if the details has the same product
        //        else
        //        {
        //            var cartDetailsFromDb = await _dBContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetailsDtos.First().ProductId
        //                                     && u.CartHeaderId == cartDto.CartHeaderDto.CartHeaderId);
        //            if (cartDetailsFromDb == null)
        //            {
        //                cartDto.CartDetailsDtos.First().CartHeaderId = cartDto.CartHeaderDto.CartHeaderId;
        //                _dBContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
        //                await _dBContext.SaveChangesAsync();
        //            }

        //            else
        //            //update count in cart details
        //            {
        //                cartDto.CartDetailsDtos.First().Count += cartDetailsFromDb.Count;
        //                //cartDto.CartDetailsDtos.First().CartHeaderId += cartDetailsFromDb.CartHeaderId;
        //                //cartDto.CartDetailsDtos.First().CartDetailsId += cartDetailsFromDb.CartDetailsId;
        //                _dBContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetailsDtos.First()));
        //                await _dBContext.SaveChangesAsync();
        //            }

        //        }
        //        _response.Result = cartDto;
        //        return _response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Error = ex.Message.ToString();
        //        return _response;
        //    }
        //}
    }
}
