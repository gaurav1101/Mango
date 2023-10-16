using IdentityModel;
using Mango.Mango.Web.Models;
using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IShoppingCartService shoppingCartService)
        {
            _logger = logger;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
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


        [HttpPost]
        public async Task<IActionResult> AddProduct(int productId,int count)
        {
            //ProductDto productDto=null;
            //var products = await _productService.GetProductByIdAsync(productId);
            //if (products.IsSuccess && products.Result != null)
            //{
            //     productDto = JsonConvert.DeserializeObject<ProductDto>(products.Result.ToString());
            //}
            List<CartDetailsDto> cartDetailsDtos = new List<CartDetailsDto>();
            CartDetailsDto cartDetails = new CartDetailsDto
            {
                ProductId = productId,
                Count = count
            };
            cartDetailsDtos.Add(cartDetails);
            CartDto cartDto = new CartDto
            {
                CartHeaderDto = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value,

                },
                CartDetailsDtos = cartDetailsDtos
            };
            var cartInfo = await _shoppingCartService.UpsertCartAsync (cartDto);
            if (cartInfo.IsSuccess && cartInfo.Result != null)
            {
                TempData["success"] = "Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = cartInfo.Error.ToString();
            return RedirectToAction(nameof(Index));
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