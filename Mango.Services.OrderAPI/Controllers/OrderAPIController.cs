using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/OrderAPI")]
    [ApiController]
    public class OrderAPIController: ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        public ResponseDto _responseDto;

        public OrderAPIController(ApplicationDBContext dBContext,IMapper mapper)
        {
            _dbContext = dBContext;
            _mapper = mapper;
            _responseDto=new ResponseDto();

        }

        //[Authorize]
        [HttpPost("CreateOrder")]
        public ActionResult<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeaderDto);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Pending;
                

               orderHeaderDto.orderDetailsdto = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetailsDtos);
                OrderHeader orderCreated = _mapper.Map<OrderHeader>(orderHeaderDto);
                  _dbContext.OrderHeaders.Add(orderCreated);
                  _dbContext.SaveChanges();
                orderHeaderDto.OrederHeaderId = orderCreated.OrederHeaderId;
                _responseDto.IsSuccess = true;
                _responseDto.Result = orderHeaderDto;
                _responseDto.StatusCode = null;
                //return _responseDto;
            }
           catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Result = ex;
                
            }
            return _responseDto;
        }


        //Process the stripe request
        [HttpPost("CreateStripeSession")]
        public ActionResult<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51OM28kSFrpBxnPQ23HGmoTOrpqMsUt27DULatAA3Ut72V1Y92j9hDeDcZQ39gLVeAS7Gqu6gVWiUzQ7kBM2pg4GC00cOCUEwpQ";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountobj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions()
                    {
                        Coupon=stripeRequestDto.orderHeaderDto.CouponCode
                    },
                };
                foreach (var item in stripeRequestDto.orderHeaderDto.orderDetailsdto)
                {
                    var sessionData = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(Convert.ToInt32(item.Price) * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                            
                        },
                        Quantity=item.Count
                        
                    };
                    options.LineItems.Add(sessionData);
                }
                if (stripeRequestDto.orderHeaderDto.Discount > 0)
                {
                    options.Discounts = discountobj;
                }
                var service = new Stripe.Checkout.SessionService();
                Session session= service.Create(options);
                stripeRequestDto.StripeSessionUrl=session.Url;
                //to sae stripe id in orderheader

                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrederHeaderId == stripeRequestDto.orderHeaderDto.OrederHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _dbContext.OrderHeaders.Update(orderHeader);
                _dbContext.SaveChanges();
                _responseDto.Result = stripeRequestDto;
            }
            catch(Exception e)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Result = e;
            }
          
            return _responseDto;
        }

        [HttpPost("ValidateStripeSession")]
        public ActionResult<ResponseDto> ValidateStripeSession([FromBody]int orderHeaderId)
        {
            try
            {
              
                //to save stripe id in orderheader
                // to insert paymnetindentid in order table and update the paymet status to succeed 
                // pymnetintendid can only be generated when payment is done.

                OrderHeader orderHeader = _dbContext.OrderHeaders.First(u => u.OrederHeaderId == orderHeaderId);
                var service = new SessionService();
                Session session=service.Get(orderHeader.StripeSessionId);
                var paymentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Approved;
                  //  _dbContext.OrderHeaders.Update(orderHeader);
                    _dbContext.SaveChanges();
                    _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
              
            }
            catch (Exception e)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Result = e;
            }

            return _responseDto;
        }
    }
}
