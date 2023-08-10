using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;
		public ProductService(IBaseService baseService, IConfiguration configuration)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto> AddProductAsync(ProductDto productDto)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.POST,
				Data = productDto,
				url = SD.ProductAPIBaseUrl + "/api/ProductAPI/",
				token = ""
			});
		}

		public async Task<ResponseDto> DeleteProductAsync(int productId)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.DELETE,
				url = SD.ProductAPIBaseUrl + "/api/ProductAPI/" + productId,
				token = ""
			});
		}

		public async Task<ResponseDto> GetAllProductsAsync()
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.GET,
				url = SD.ProductAPIBaseUrl + "/api/ProductAPI/" ,
				token = ""
			});
		}

		public async Task<ResponseDto> GetProductByIdAsync(int productId)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.GET,
				Data = productId,
				url = SD.ProductAPIBaseUrl + "/api/ProductAPI/" + productId,
				token = ""
			});
		}

		public async Task<ResponseDto> UpdateProductAsync(int productId, ProductDto productDto)
		{
			return await _baseService.sendAsync(new Models.Dto.RequestDto()
			{
				ApiType = SD.ApiTypes.PUT,
				Data = productDto,
				url = SD.ProductAPIBaseUrl + "/api/ProductAPI/" + productId,
				token = ""
			});

		}
	}
}
