using System.Collections.Generic;
using System.Drawing;
using Chat.API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Chat.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {

        private readonly ILogger<MessageController> _logger;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMessages")]
        public IEnumerable<MessageDTO> Get([FromQuery] int lastCount)
        {
            return new[] {new MessageDTO("Nick", "Hello world", Color.Aqua)};
        }
    }
}