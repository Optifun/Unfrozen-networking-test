﻿using System.Collections.Generic;
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
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class Chat
    {
        private ChatClient _client;
        private readonly ChatUI _chatUI;
        private UserDTO _user;

        public Chat(ChatUI chatUI)
        {
            _chatUI = chatUI;
            _chatUI.OnLogin += Login;
            _chatUI.OnSend += SendMessage;
        }

        private async void Login(IPAddress ipAddress, string username, Color color)
        {
            UserDTO user = await SendAuthRequest(ipAddress, username, ToARGB(color));
            if (user != null)
            {
                _user = user;
                _chatUI.EnterChat();
                await JoinChat(ipAddress);
            }
        }

        private Task JoinChat(IPAddress address)
        {
            _client = new ChatClient(address);
            _client.LastMessagesReceived += PopulateMessages;
            _client.MessageReceived += PrintMessage;

            return _client.Connect();
        }

        private async void SendMessage(string text)
        {
            var message = new MessageDTO(_user.Name, text, _user.Color);
            await _client.SendMessageAsync(WSMessage.SendMessage, message);
            PrintMessage(message);
        }

        private async UniTask<UserDTO> SendAuthRequest(IPAddress ipAddress, string username, int color)
        {
            string json = JsonConvert.SerializeObject(new UserDTO(username, color));

            UnityWebRequest request = await UnityWebRequest
                .Put($"http://{ipAddress}:8081/Auth", Encoding.Default.GetBytes(json))
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Accept", "application/json")
                .SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                return JsonConvert.DeserializeObject<UserDTO>(request.downloadHandler.text);

            return null;
        }

        private int ToARGB(Color color)
        {
            int ToHex(float value) =>
                (int) (value * 255);

            return System.Drawing.Color
                .FromArgb(ToHex(color.a), ToHex(color.r), ToHex(color.g), ToHex(color.b))
                .ToArgb();
        }

        private void PrintMessage(MessageDTO message)
        {
            _chatUI.Print(message);
        }

        private void PopulateMessages(List<MessageDTO> messages)
        {
            foreach (var messageDto in messages)
                _chatUI.Print(messageDto);
        }
    }
}