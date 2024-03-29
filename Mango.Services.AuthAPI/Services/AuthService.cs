﻿using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Services
{
    public class AuthService: IAuthService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthService(ApplicationDBContext applicationDBContext,UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, RoleManager<IdentityRole> roleManager) 
        { 
            _dbContext = applicationDBContext;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
           _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string role)
        {
            var user= _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if(user != null) 
            {
                if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                   await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user,role);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.Username.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user != null && isValid )
            {
                var roles = await _userManager.GetRolesAsync(user);
                LoginResponseDto loginResponseDto = new()
                {
                    User = new()
                    {

                        Email = user.Email,
                        Name = user.Name,
                    },
                    token = _jwtTokenGenerator.GenerateToken(user, roles)

                };
                return loginResponseDto;
            }
                LoginResponseDto loginResponse1 = new LoginResponseDto
                {
                    token = "",  //to convert token to string
                    User = null
                };
                return loginResponse1;
        }

        public async Task<ResponseDto> Register(RegisterationRequestDto registerationRequestDto)
        {
            ResponseDto responseDto = new ResponseDto();
            ApplicationUser _user =
                   _user = new ApplicationUser
                   {
                       UserName = registerationRequestDto.UserName,
                       Email = registerationRequestDto.Email,
                       NormalizedEmail = registerationRequestDto.Email.ToUpper(),
                       Name = registerationRequestDto.Name,
                   };
            try
            {
                var response = await _userManager.CreateAsync(_user, registerationRequestDto.Password);
                if (response.Succeeded)
                {
                    //if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    //    await _roleManager.CreateAsync(new IdentityRole("customer"));
                    //}
                    var userReturn = _dbContext.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == _user.UserName.ToLower());
                    if (userReturn != null)
                    {
                        responseDto = new()
                        {
                            Result = userReturn
                        };
                    return responseDto;
                    }
                }
                else
                {
                    responseDto = new ResponseDto()
                    {
                        Error = response.Errors.First().Description,
                        IsSuccess = false
                    };
                    return responseDto;
                }
                
            }
            catch (Exception ex)
            {

            }
            return responseDto;
        }
    }
}
