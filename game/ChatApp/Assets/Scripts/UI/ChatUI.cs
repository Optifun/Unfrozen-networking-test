using System;
using Chat.Shared.DTO;
using UnityEngine;

namespace UI
{
    public class ChatUI : MonoBehaviour
    {
        public event Action<string, Color> OnLogin;
        public event Action<string> OnSend;

        public void Print(MessageDTO message)
        {
            
        }

        public void EnterChat()
        {
            throw new NotImplementedException();
        }
    }
}