using Azure;
using Mango.ServiceBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ResponseDto _response;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public AuthAPIController(IAuthService authService, IMessageBus messageBus,IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration= configuration;
            _response = new ResponseDto();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterationRequestDto registerationRequestDto)
        {
            var user = await _authService.Register(registerationRequestDto);
            if (user.Result != null)
            {
                await _messageBus.publishMessage("Test Email",_configuration.GetValue<string>("ServiceBusConfig:ServiecBusName"));
                _response.StatusCode = HttpStatusCode.OK.ToString();
                _response.Result = user.Result;
                _response.IsSuccess = true;
                return Ok(_response);
               
            }
            _response.Error = user.Error;
            _response.IsSuccess = false;
            return Ok(_response);

        }

        [HttpPost("Login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var loginUser = await _authService.Login(loginRequest);
            if (loginUser.User == null && loginUser.token == "")
            {
                _response.Error = "Username or Password is incorrect" ;
                _response.IsSuccess = false;
                return Ok(_response);
            }
            _response.StatusCode = HttpStatusCode.OK.ToString();
            _response.Result = loginUser;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDto registerationRequestDto)
        {
            var isAssign = await _authService.AssignRole(registerationRequestDto.Email, registerationRequestDto.Role.ToUpper());
            if (!isAssign)
            {
                _response.Error = "Role not assigned";
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK.ToString();
            _response.Result = isAssign;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}

