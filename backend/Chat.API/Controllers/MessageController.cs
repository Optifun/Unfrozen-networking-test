using System.Collections.Generic;
using System.Drawing;
using Chat.API.DataAccess;
using Chat.API.Services;
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
        private readonly MessageRepository _messageRepository;

        public MessageController(ILogger<MessageController> logger, MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        [HttpGet("GetMessages")]
        public IEnumerable<MessageDTO> Get([FromQuery] int lastCount)
        {
            return _messageRepository
                .GetLast(lastCount)
                .Adapt<List<MessageDTO>>();
        }
    }
}