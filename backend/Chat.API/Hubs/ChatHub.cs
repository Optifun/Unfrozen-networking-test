using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Chat.Shared.DTO;
using Chat.API.Services;
using Chat.Shared;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(MessageRepository messageRepository, ILogger<ChatHub> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await FetchLastMessages();
        }

        [HubMethodName(nameof(WSMessage.SendMessage))]
        public async Task SendMessage(MessageDTO message)
        {
            try
            {
                await _messageRepository.Create(message.Username, message.Text, message.Color);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "Not existing user sent a message");
                return;
            }

            await Clients.Others.SendAsync(WSMessage.Receive.ToString(), message);
        }

        private async Task FetchLastMessages()
        {
            List<MessageDTO> messages = (await _messageRepository.GetLast(20)).Adapt<List<MessageDTO>>();
            await Clients.Caller.SendAsync(WSMessage.ReceiveLast20.ToString(), messages);
        }
    }
}