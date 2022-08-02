using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chat.Shared;
using Chat.Shared.DTO;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client
{
    public class ChatClient : IAsyncDisposable
    {
        private readonly HubConnection _connection;

        public event Action<UserDTO> UserJoined;
        public event Action<UserDTO> UserQuit;
        public event Action<List<MessageDTO>> LastMessagesReceived;
        public event Action<MessageDTO> MessageReceived;
        public event Func<Exception, Task> Closed;
        public event Func<string, Task> Reconnected;
        public event Func<Exception, Task> Reconnecting;

        public ChatClient(IPAddress address, Cookie authCookie)
        {
            _connection = new HubConnectionBuilder().WithUrl($"http://{address}:8081/ws/chat", options =>
            {
                options.Cookies.Add(authCookie);
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            }).Build();

            _connection.Closed += OnClosed;
            _connection.Reconnected += OnReconnected;
            _connection.Reconnecting += OnReconnecting;
            _connection.On(WSMessage.ReceiveLast20.ToString(), (IEnumerable<MessageDTO> messages) => { LastMessagesReceived?.Invoke(messages.ToList()); });
            _connection.On(WSMessage.UserQuit.ToString(), (UserDTO user) => UserQuit?.Invoke(user));
            _connection.On(WSMessage.UserJoined.ToString(), (UserDTO user) => UserJoined?.Invoke(user));
            _connection.On(WSMessage.Receive.ToString(), (MessageDTO message) => { MessageReceived?.Invoke(message); });
        }

        public Task Connect() =>
            _connection.StartAsync();

        public Task SendMessageAsync(WSMessage messageType, object arg) =>
            _connection.SendAsync(messageType.ToString(), arg);

        public Task<ChannelReader<UserDTO>> FetchUsers(int pageSize = 10) =>
            _connection.StreamAsChannelAsync<UserDTO>(WSMessage.StreamAllUsers.ToString(), pageSize);

        public Task<ChannelReader<UserDTO>> FetchOnlineUsers() =>
            _connection.StreamAsChannelAsync<UserDTO>(WSMessage.StreamOnlineUsers.ToString());

        public Task Disconnect() =>
            _connection.StopAsync();

        private Task OnReconnecting(Exception ex) =>
            Reconnecting?.Invoke(ex);

        private Task OnReconnected(string connectionId) =>
            Reconnected?.Invoke(connectionId);

        private Task OnClosed(Exception ex) =>
            Closed?.Invoke(ex);

        public ValueTask DisposeAsync()
        {
            Disconnect();
            return _connection.DisposeAsync();
        }
    }
}