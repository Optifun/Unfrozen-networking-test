using System;
using System.Net;
using Chat.Shared.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChatUI : MonoBehaviour
    {
        public event Action<IPAddress, string, Color> OnLogin;
        public event Action<string> OnSend;

        public MessageBubble MessagePrefab;

        [SerializeField] private Canvas LoginCanvas;
        [SerializeField] private Canvas ChatCanvas;


        [SerializeField] private TMP_InputField hostText;
        [SerializeField] private TMP_InputField nameText;
        [SerializeField] private TMP_InputField colorText;
        [SerializeField] private Button loginButton;


        [SerializeField] private TMP_InputField messageText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Transform messageContainer;


        private void Awake()
        {
            loginButton.onClick.AddListener(TryLogin);
            sendButton.onClick.AddListener(Send);
        }

        private void TryLogin()
        {
            try
            {
                IPAddress address = hostText.text == "localhost" ? IPAddress.Loopback : IPAddress.Parse(hostText.text);
                if (nameText.text == "")
                    throw new ArgumentException("Empty name specified");

                OnLogin?.Invoke(address, nameText.text, GetColor(colorText.text));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void Send() =>
            OnSend?.Invoke(messageText.text);

        public void Print(MessageDTO message)
        {
            MessageBubble messageView = Instantiate(MessagePrefab, messageContainer);
            messageView.Initialize(message.Username, message.Text, System.Drawing.Color.FromArgb((int) message.Color).ToUColor());
        }

        public void EnterChat()
        {
            LoginCanvas.enabled = false;
            ChatCanvas.enabled = true;
        }

        public void EnterLobby()
        {
            LoginCanvas.enabled = true;
            ChatCanvas.enabled = false;
        }

        private Color GetColor(string text) =>
            text.ToLower() switch
            {
                "red" => Color.red,
                "blue" => Color.blue,
                "pink" => Color.magenta,
                "green" => Color.green,
                _ => throw new ArgumentException("Invalid color specified")
            };
    }
}