using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Chat.API.Services;
using Chat.Shared.DTO;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        [HttpPut]
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
                new AuthenticationProperties()
                {
                    IsPersistent = false
                });

            return userDb.Adapt<UserDTO>();
        }
    }
}