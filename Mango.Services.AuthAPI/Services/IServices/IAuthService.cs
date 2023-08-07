using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto> Register(RegisterationRequestDto registerRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email,string role);
    }
}
