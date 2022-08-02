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
            _chat = new Chat(_chatUI);
            _chatUI.EnterLobby();
        }

        private void OnDestroy()
        {
            _chat.Dispose();
        }
    }
}