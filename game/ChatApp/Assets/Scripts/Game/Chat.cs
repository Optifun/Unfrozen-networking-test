using System.Collections.Generic;
using System.Net;
using Chat.Client;
using Chat.Shared;
using Chat.Shared.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class Chat
    {
        private readonly ChatClient _client;
        private readonly ChatUI _chatUI;
        private UserDTO _user;

        public Chat(ChatClient client, ChatUI chatUI)
        {
            _client = client;
            _chatUI = chatUI;

            _client.LastMessagesReceived += PopulateMessages;
            _client.MessageReceived += PrintMessage;

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
                await _client.Connect();
            }
        }

        private void SendMessage(string text)
        {
            var message = new MessageDTO(_user.Name, text, _user.Color);
            _client.SendMessageAsync(WSMessage.SendMessage, message);
            PrintMessage(message);
        }

        private async UniTask<UserDTO> SendAuthRequest(IPAddress ipAddress, string username, int color)
        {
            UnityWebRequest request = await UnityWebRequest
                .Post($"http://{ipAddress}/auth", JsonConvert.SerializeObject(new UserDTO(username, color)))
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