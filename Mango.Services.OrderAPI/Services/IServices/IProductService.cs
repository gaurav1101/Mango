using Mango.Services.OrderAPI.Models.Dtos;

namespace Mango.Services.OrderAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
