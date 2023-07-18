using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        // GET: AuthController
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var user= await _authService.LoginAsync(loginRequestDto);
            var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(user.Result));
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwt = handler.ReadJwtToken(loginResponseDto.token);

                //To enable auth , after program.cs use below code
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                //identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                //identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                if (loginResponseDto != null)
                {
                    return RedirectToAction("CouponIndex", "Coupon");
                }
                return View();
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        // GET: AuthController/Details/5
        public IActionResult Register(RegisterationRequestDto registerationRequestDto)
        {
            var user= _authService.RegisterAsync(registerationRequestDto);
            if (user != null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }


        [HttpPost]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
