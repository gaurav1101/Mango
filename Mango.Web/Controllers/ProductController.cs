using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
		private readonly ITokenProvider _tokenProvider;
		ResponseDto _response;
        public ProductController(IProductService productService)
        {
			_productService = productService;
             _response = new ResponseDto();
            
        }

        [HttpGet]
        public async Task<IActionResult> ProductIndex()
        {
            try
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
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        [Authorize()]
        public async Task<IActionResult> CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var products = await _productService.AddProductAsync(productDto);
                    if (products.IsSuccess && products.Result != null)
                    {
                        TempData["success"] = "Created Successfully";
                        return RedirectToAction(nameof(ProductIndex));
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

		[Authorize()]
        public async Task<IActionResult> UpdateProduct(int productId)
		{
            try
            {
                if (productId > 0)
                {
                    var product = await _productService.GetProductByIdAsync (productId);
                    if(product.Result != null && product.IsSuccess==true)
                    {
                       ProductDto productDto= JsonConvert.DeserializeObject<ProductDto>(product.Result.ToString());

                        return View(productDto);
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


		[Authorize()]
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDto productDto)
		{
            var product = await _productService.UpdateProductAsync(productDto.ProductId, productDto);
            if(product.IsSuccess==true && product.Result != null)
            {
                TempData["Success"] = "Product Updated Successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            TempData["Error"] = "Product Not Updated";
            return View();
		}

		[Authorize()]
        public async Task<IActionResult> RemoveProduct(int productId)
        {
            var product=await _productService.GetProductByIdAsync(productId);
            if(product.IsSuccess==true && product.Result != null)
            {
                ProductDto productDto=JsonConvert.DeserializeObject<ProductDto>(product.Result.ToString());
                return View(productDto);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveProduct(ProductDto productDto)
        {
            try
            {
                    var product = await _productService.DeleteProductAsync(productDto.ProductId);
                    if (product.IsSuccess && product.Result != null)
                    {
                    TempData["success"] ="Deleted Successfully";
                    return RedirectToAction(nameof(ProductIndex));
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
