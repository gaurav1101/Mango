using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IService
{
	public interface IProductService
	{
		Task<ResponseDto> GetProductByIdAsync(int productId);
		Task<ResponseDto> GetAllProductsAsync();
		Task<ResponseDto> AddProductAsync(ProductDto productDto);
		Task<ResponseDto> UpdateProductAsync(int productId, ProductDto productDto);
		Task<ResponseDto> DeleteProductAsync(int productId);
	}
}
