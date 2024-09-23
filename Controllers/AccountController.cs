using System.Text;
using api.Dtos.Accounts;
using API.Dtos.Accounts;
using API.Interfaces.Services;
using API.Models.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]s")]
    public class AccountController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);

                if (user == null) return Unauthorized("Username not found or password incorrect");

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, false);

                if (!result.Succeeded)
                    return Unauthorized("Username not found or password incorrect");

                HttpContext.Request.Headers.TryGetValue("Authorization", out var tokenStr);

                string token = tokenStr.FirstOrDefault() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(token) || !await _userManager.VerifyTwoFactorTokenAsync(user, "CustomTokenProvider", token))
                    token = await _userManager.GenerateTwoFactorTokenAsync(user, "CustomTokenProvider");

                if (token.StartsWith("Bearer"))
                    token = token.Substring(7);

                return Ok(
                    new LoginUserDto
                    {
                        UserName = loginDto.UserName!,
                        Email = user.Email!,
                        Token = token
                    }
                );
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            return await Register(registerDto, "Admin");
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            return await Register(registerDto, "User");
        }

        private async Task<IActionResult> Register([FromBody] RegisterDto registerDto, string role = "User")
        {
            try
            {
                if (string.IsNullOrEmpty(role))
                    return BadRequest($"Role not found: {role}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (await _userManager.FindByEmailAsync(registerDto.Email!) != null)
                    return BadRequest("Email exists. Please choose a new email!");

                var newAppUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(newAppUser, registerDto.Password ?? throw new Exception("Password is not provided"));

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(newAppUser, role);
                    if (roleResult.Succeeded)
                        return Ok(new NewUserDto
                        {
                            UserName = newAppUser.UserName!,
                            Email = newAppUser.Email!,
                            Token = await _userManager.GenerateTwoFactorTokenAsync(newAppUser, "CustomTokenProvider")
                        });
                    else
                        return StatusCode(500, roleResult.Errors);
                }
                else return StatusCode(500, createdUser.Errors);
            }
            catch (Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}