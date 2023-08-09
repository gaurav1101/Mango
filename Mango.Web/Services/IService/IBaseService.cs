using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IService
{
    public interface IBaseService
    {
        Task<ResponseDto> sendAsync(RequestDto requestDto,bool withBearer=true);
    }
}
