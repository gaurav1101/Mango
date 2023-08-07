using Mango.Web.Models.Dto;
using Mango.Web.Services.IService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mango.Web.Utility;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService,ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
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
            
            try
            {
                if (user.Result != null && user.IsSuccess)
                {
                    var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(user.Result));
                    await SignInUser(loginResponseDto);
                    _tokenProvider.setToken(loginResponseDto.token); // Set token value in the cookies
                    return RedirectToAction("CouponIndex", "Coupon");
                }

                ModelState.AddModelError("CustomError", user.Error.ToString());
                return View(loginRequestDto);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("CustomError", user.Error.ToString());
                return View(loginRequestDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem{ Text=SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text=SD.RoleCostumer, Value= SD.RoleCostumer}
            };
            ViewBag.RoleList = roleList;
                return View();
        }

        [HttpPost]
        // GET: AuthController/Details/5
        public async Task<IActionResult> Register(RegisterationRequestDto registerationRequestDto)
        {
            var user= _authService.RegisterAsync(registerationRequestDto);
            
            if (user != null && user.Result.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerationRequestDto.Role))
                {
                    registerationRequestDto.Role=SD.RoleCostumer;
                }
                var assignRole = _authService.AssignRoleAsync(registerationRequestDto);
                if (assignRole.Result != null && assignRole.Result.IsSuccess)
                {
                    TempData["Success"] = "Registration Successfull";
                }
                return RedirectToAction(nameof(Login));
            }
            var roleList = new List<SelectListItem>
            {
                new SelectListItem{ Text=SD.RoleAdmin, Value = SD.RoleAdmin },
                new SelectListItem{ Text=SD.RoleCostumer, Value= SD.RoleCostumer}
            };
            ViewBag.RoleList = roleList;
            return View(registerationRequestDto);
        }


        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
             _tokenProvider.clearToken();
            return RedirectToAction(nameof(Index),"Home");
        }

        //To signedIn using .net Identity
        // This is created to check whether the user is sigedIn or not because 
        //we are using, it in _layout.cshtml !User.Identity.IsAuthenticated
        public async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.ReadJwtToken(loginResponseDto.token); // Read the token from loginResponseDto
            
            //To signIn user user .net Identity 
            // these are the basic and default steps that needs to e followed
            var identity=new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email,
                token.Claims.FirstOrDefault(u => u.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                token.Claims.FirstOrDefault(u => u.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name,
                token.Claims.FirstOrDefault(u => u.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                token.Claims.FirstOrDefault(u => u.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                token.Claims.FirstOrDefault(u => u.Type =="role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
