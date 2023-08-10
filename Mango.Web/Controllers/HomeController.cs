using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
                IEnumerable<ProductDto> productDtos;
                var products = await _productService.GetAllProductsAsync();
                if (products.IsSuccess && products.Result != null)
                {
                    productDtos = JsonConvert.DeserializeObject<List<ProductDto>>(products.Result.ToString());
                    return View(productDtos);
                }
                TempData["error"] = products.Error.ToString();
                productDtos = new List<ProductDto>();
                return View(productDtos);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {
            var products = await _productService.GetProductByIdAsync(productId);
            if (products.IsSuccess && products.Result != null)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(products.Result.ToString());
                return View(productDto);
            }
            TempData["error"] = products.Error.ToString();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}