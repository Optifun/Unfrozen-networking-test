using System;
using Chat.Shared.DTO;
using UnityEngine;

namespace UI
{
    public class ChatUI : MonoBehaviour
    {
        public event Action<string, Color> OnLogin;
        public event Action<string> OnSend;

        public MessageBubble MessagePrefab;

        [SerializeField] private Canvas LoginCanvas;
        [SerializeField] private Canvas ChatCanvas;

        [SerializeField] private Transform messageContainer;

        public void Print(MessageDTO message)
        {
            MessageBubble messageView = Instantiate(MessagePrefab, messageContainer);
            messageView.Initialize(message.Username, Color.black, message.Text);
        }

        public void EnterChat()
        {
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
        }
    }
}