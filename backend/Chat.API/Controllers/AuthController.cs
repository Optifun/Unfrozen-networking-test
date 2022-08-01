using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Chat.API.Services;
using Chat.Shared.DTO;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public AuthController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPut("login")]
        public async Task<ActionResult<UserDTO>> AuthUser([FromBody] UserDTO user)
        {
            User userDb = await _userRepository.GetOrAdd(user.Name, user.Color);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, userDb.Name),
                new("Color", userDb.Color.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties());

            return userDb.Adapt<UserDTO>();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return Ok("success");
        }

        [Authorize]
        [HttpGet]
        [Route("user-profile")]
        public async Task<ActionResult<UserDTO>> UserProfileAsync()
        {
            var nameClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            var colorClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Color");

            User user = await _userRepository.Get(nameClaim.Value, long.Parse(colorClaim.Value));

            return Ok(user);
        }
    }
}