using Azure;
using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            return View();
        }

        public async  Task<IActionResult> OrderDetails(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            ResponseDto responseDto = await _orderService.GetOrderByIdAsync(orderId);
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            if (responseDto != null)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(responseDto.Result.ToString());
            }
            else if(userId!= orderHeaderDto.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        [HttpGet]
        public IActionResult getOrders()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto response =  _orderService.GetOrdersAsync(userId).GetAwaiter().GetResult();
            IEnumerable<OrderHeaderDto> orderHeaderDto;
            if (response != null && response.IsSuccess)
            {
               orderHeaderDto= JsonConvert.DeserializeObject<List<OrderHeaderDto>>(response.Result.ToString());
			   
			}
            else
            {
                orderHeaderDto = new List<OrderHeaderDto>();
                
            }
          var json= Json(new { data = orderHeaderDto });
            return json;
           
        }
    }
}
