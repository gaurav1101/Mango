using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using System;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.sendAsync(new RequestDto
            {
                Data = loginRequestDto,
                ApiType = SD.ApiTypes.POST,
                url = SD.AuthAPIBaseUrl + "/api/auth/Login"
            });
        }

        public async Task<ResponseDto> RegisterAsync(RegisterationRequestDto registerationRequestDto)
        {
            return await _baseService.sendAsync(new RequestDto
            {
                Data = registerationRequestDto,
                ApiType = SD.ApiTypes.POST,
                url = SD.AuthAPIBaseUrl + "/api/auth/Register"
            });
        }

        public async Task<ResponseDto> AssignRoleAsync(RegisterationRequestDto registerationRequestDto)
        {
            return await _baseService.sendAsync(new RequestDto
            {
                Data = registerationRequestDto,
                ApiType = SD.ApiTypes.POST,
                url = SD.AuthAPIBaseUrl + "/api/auth/AssignRole"
            });
        }
    }
}
