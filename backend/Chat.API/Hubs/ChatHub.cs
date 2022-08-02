using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Chat.Shared.DTO;
using Chat.API.Services;
using Chat.Shared;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Chat.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly ILogger<ChatHub> _logger;
        private readonly UserRepository _userRepository;
        private readonly IChatUsers _usersCollection;
        private UserDTO? _user;

        public ChatHub(MessageRepository messageRepository, UserRepository userRepository, IChatUsers usersCollection, ILogger<ChatHub> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
            _usersCollection = usersCollection;
            _userRepository = userRepository;
        }

        public override async Task OnConnectedAsync()
        {
            if (!GetUserInfo(out UserDTO? user)) return;

            _user = user;
            _usersCollection.Add(_user);

            await Clients.Others.SendAsync(WSMessage.UserJoined.ToString(), _user);
            await FetchLastMessages();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception == null)
                _logger.LogInformation($"User {_user} disconnected from hub");
            else
                _logger.LogError($"User {_user} disconnected from hub with exception {exception}");

            if (_user != null)
            {
                _usersCollection.Remove(_user);
                await Clients.Others.SendAsync(WSMessage.UserQuit.ToString(), _user);
            }
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

        [HubMethodName(nameof(WSMessage.StreamAllUsers))]
        public async Task<ChannelReader<UserDTO>> GetAllUsers(int pageSize, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(WSMessage.StreamAllUsers)} called");
            
            int usersCount = await _userRepository.CountUsers();
            Channel<UserDTO> channel = Channel.CreateBounded<UserDTO>(usersCount);

            for (int pageNum = 0; pageNum <= usersCount / pageSize; pageNum++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                UserDTO[] users = await _userRepository.GetUsers(pageNum, pageSize);
                foreach (UserDTO user in users)
                    await channel.Writer.WriteAsync(user, cancellationToken);
            }

            return channel.Reader;
        }

        [HubMethodName(nameof(WSMessage.StreamOnlineUsers))]
        public async Task<ChannelReader<UserDTO>> GetOnlineUsers()
        {
            _logger.LogInformation($"{nameof(WSMessage.StreamOnlineUsers)} called");
            
            List<UserDTO> users = _usersCollection.GetUsers();
            Channel<UserDTO> channel = Channel.CreateBounded<UserDTO>(users.Count);
            
            foreach (UserDTO user in users)
                await channel.Writer.WriteAsync(user);

            return channel.Reader;
        }

        private async Task FetchLastMessages()
        {
            List<MessageDTO> messages = (await _messageRepository.GetLast(20)).Adapt<List<MessageDTO>>();
            await Clients.Caller.SendAsync(WSMessage.ReceiveLast20.ToString(), messages);
        }

        private bool GetUserInfo(out UserDTO? user)
        {
            Claim? nameClaim = Context.User?.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
            Claim? colorClaim = Context.User?.Claims.FirstOrDefault(claim => claim.Type == "Color");

            if (nameClaim == null || colorClaim == null)
            {
                _logger.LogInformation($"Failed to login user={Context.ConnectionId}");
                user = null;
                return false;
            }

            user = new UserDTO(nameClaim.Value, long.Parse(colorClaim.Value));
            return true;
        }
    }
}