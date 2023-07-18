using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.IService
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterationRequestDto registerationRequestDto);
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto> AssignRoleAsync(RegisterationRequestDto registerationRequestDto);
    }
}
