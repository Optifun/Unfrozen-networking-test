using System.Collections.Generic;
using System.Drawing;
using Chat.API.DataAccess;
using Chat.Shared.DTO;
using Mapster;
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

        [HttpGet("GetMessages")]
        public IEnumerable<MessageDTO> Get([FromQuery] int lastCount)
        {
            return new Message[]
            {
                new()
                {
                    Text = "Hello world",
                    User = new()
                    {
                        Name = "Nick",
                        Color = Color.Aqua.ToArgb()
                    }
                }
            }.Adapt<IEnumerable<MessageDTO>>();
        }
    }
}