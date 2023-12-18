using Mango.Web.Models;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class OrderController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        //public IActionResult Create(CartDto cartDto)
       // {
           //var response=_orderService.CreateOrderAsync(cartDto);
            //if (response != null)
            //{
                //return View();
            //}
        //}
    }
}
