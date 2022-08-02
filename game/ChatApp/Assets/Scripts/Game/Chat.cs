using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Chat.Client;
using Chat.Shared;
using Chat.Shared.DTO;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Newtonsoft.Json;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class Chat : IDisposable
    {
        private ChatClient _client;
        private readonly ChatUI _chatUI;
        private UserDTO _user;
        private readonly ReactiveCollection<UserDTO> _users = new();
        private readonly ReactiveCollection<UserDTO> _onlineUsers = new();

        public Chat(ChatUI chatUI)
        {
            _chatUI = chatUI;
            _chatUI.OnLogin += Login;
            _chatUI.OnSend += SendMessage;
            _users.ObserveAdd().Subscribe(e => _chatUI.AddUser(e.Value));
            _onlineUsers.ObserveAdd().Subscribe(e => _chatUI.AddOnlineUser(e.Value));
            _onlineUsers.ObserveRemove().Subscribe(e => _chatUI.RemoveOnlineUser(e.Value));
        }

        private async void Login(IPAddress ipAddress, string username, Color color)
        {
            (UserDTO user, Cookie cookie) = await SendAuthRequest(ipAddress, username, ToARGB(color));
            if (user != null)
            {
                _user = user;
                _users.Clear();
                _onlineUsers.Clear();
                
                _chatUI.EnterChat();
                await JoinChat(ipAddress, cookie);
                await FetchUsers();
                await FetchOnlineUsers();
            }
        }

        private async Task FetchUsers()
        {
            var channel = await _client.FetchUsers(10);
            List<UserDTO> users = await channel.ToListAsync();
            _users.AddRange(users);
            Debug.Log("Users loaded");
        }

        private async Task FetchOnlineUsers()
        {
            var channel = await _client.FetchOnlineUsers();
            List<UserDTO> users = await channel.ToListAsync();
            _onlineUsers.AddRange(users);
            Debug.Log("OnlineUsers loaded");
        }

        private Task JoinChat(IPAddress address, Cookie cookie)
        {
            _client = new ChatClient(address, cookie);
            _client.LastMessagesReceived += PopulateMessages;
            _client.MessageReceived += PrintMessage;
            _client.UserQuit += OnUserQuit;
            _client.UserJoined += OnUserJoined;

            _client.Reconnecting += async exception => Debug.LogException(exception);
            _client.Closed += async exception => Debug.LogException(exception);

            return _client.Connect();
        }

        private async void SendMessage(string text)
        {
            var message = new MessageDTO(_user.Name, text, _user.Color);
            await _client.SendMessageAsync(WSMessage.SendMessage, message);
            PrintMessage(message);
        }


        private async UniTask<(UserDTO, Cookie)> SendAuthRequest(IPAddress ipAddress, string username, int color)
        {
            string json = JsonConvert.SerializeObject(new UserDTO(username, color));

            UnityWebRequest request = await UnityWebRequest
                .Put($"http://{ipAddress}:8081/Auth/login", Encoding.Default.GetBytes(json))
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Accept", "application/json")
                .SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string authCookie = request.GetResponseHeader("Set-Cookie");
                int start = authCookie.IndexOf("=") + 1;
                int end = authCookie.IndexOf(";");
                string token = authCookie.Substring(start, end - start);
                var cookie = new Cookie(".AspNetCore.Cookies", token, "/", ipAddress.ToString());
                return (JsonConvert.DeserializeObject<UserDTO>(request.downloadHandler.text), cookie);
            }

            return (null, null);
        }

        private int ToARGB(Color color) =>
            color.ToColor().ToArgb();

        private void PrintMessage(MessageDTO message)
        {
            _chatUI.Print(message);
        }

        private void PopulateMessages(List<MessageDTO> messages)
        {
            foreach (var messageDto in messages)
                _chatUI.Print(messageDto);
        }

        private void OnUserJoined(UserDTO user) =>
            _onlineUsers.Add(user);

        private void OnUserQuit(UserDTO user) =>
            _onlineUsers.Remove(user);

        public void Dispose()
        {
            _client?.DisposeAsync();
        }
    }
}