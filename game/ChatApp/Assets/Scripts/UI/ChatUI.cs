using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Chat.Shared.DTO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace UI
{
    public class ChatUI : MonoBehaviour
    {
        public event Action<IPAddress, string, Color> OnLogin;
        public event Action<string> OnSend;

        public MessageBubble MessagePrefab;
        public UserView UserViewPrefab;

        [SerializeField] private Canvas LoginCanvas;
        [SerializeField] private Canvas ChatCanvas;


        [SerializeField] private TMP_InputField hostText;
        [SerializeField] private TMP_InputField nameText;
        [SerializeField] private TMP_InputField colorText;
        [SerializeField] private Button loginButton;


        [SerializeField] private TMP_InputField messageText;
        [SerializeField] private Button sendButton;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private Transform usersContainer;
        [SerializeField] private Transform onlineListStart;
        [SerializeField] private Transform offlineListStart;

        private readonly List<UserView> _views = new();


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
            foreach (UserView view in _views)
                Destroy(view.gameObject);

            _views.Clear();
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

        public void AddUser(UserDTO user) =>
            ShowUser(user, false);

        public void AddOnlineUser(UserDTO user)
        {
            UserView? userView = _views.FirstOrDefault(view => view.User == user);
            if (userView)
            {
                userView.SetOnline(false);
                PlaceUnder(userView.transform, onlineListStart);
            }
            else
            {
                ShowUser(user, true);
            }
        }

        public void RemoveOnlineUser(UserDTO user)
        {
            UserView? userView = _views.FirstOrDefault(view => view.User == user);
            if (userView)
            {
                userView.SetOnline(false);
                PlaceUnder(userView.transform, offlineListStart);
            }
            else
            {
                ShowUser(user, false);
            }
        }

        private UserView ShowUser(UserDTO user, bool online)
        {
            UserView userView = Instantiate(UserViewPrefab, usersContainer);

            PlaceUnder(userView.transform,
                online ? onlineListStart : offlineListStart);

            userView.Initialize(user, online);
            _views.Add(userView);
            return userView;
        }

        private void PlaceUnder(Transform target, Transform under) =>
            target.SetSiblingIndex(under.GetSiblingIndex() + 1);
    }
}