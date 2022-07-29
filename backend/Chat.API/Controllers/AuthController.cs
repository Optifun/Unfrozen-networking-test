using System.Threading.Tasks;
using Chat.API.DataAccess;
using Chat.API.Services;
using Chat.Shared.DTO;
using Mapster;
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

        [HttpPost]
        public async Task<ActionResult<UserDTO>> AuthUser([FromBody] UserDTO user)
        {
            User userDb = await  _userRepository.GetOrAdd(user.Name, user.Color);
            return userDb.Adapt<UserDTO>();
        }
    }
}