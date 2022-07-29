using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Shared;
using Chat.Shared.DTO;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client
{
    public class ChatClient
    {
        private readonly HubConnection _connection;

        public event Action<List<MessageDTO>> LastMessagesReceived;
        public event Action<MessageDTO> MessageReceived;

        public ChatClient()
        {
            _connection = new HubConnectionBuilder().WithUrl("ws://localhost/ws/chat").Build();

            _connection.On(WSMessage.ReceiveLast10.ToString(), (IEnumerable<MessageDTO> messages) => { LastMessagesReceived?.Invoke(messages.ToList()); });

            _connection.On(WSMessage.Receive.ToString(), (MessageDTO message) => { MessageReceived?.Invoke(message); });
        }

        public Task Connect() => 
            _connection.StartAsync();

        public Task SendMessageAsync(WSMessage messageType, object arg) => 
            _connection.SendAsync(messageType.ToString(), arg);

        public Task Disconnect() => 
            _connection.StopAsync();
    }
}