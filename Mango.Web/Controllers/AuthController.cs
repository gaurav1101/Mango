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
            
            try
            {
                if (loginResponseDto != null)
                {
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


        [HttpPost]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
