using System;
using Chat.Client;
using UI;
using UnityEngine;

namespace Game
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ChatUI _chatUI;
        private Chat _chat;

        private void Start()
        {
            ChatClient client = new ChatClient();
            _chat = new Chat(client, _chatUI);
            _chatUI.EnterLobby();
        }
    }
}