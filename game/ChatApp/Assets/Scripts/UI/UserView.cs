using Chat.Shared.DTO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Color = System.Drawing.Color;
using UColor = UnityEngine.Color;

namespace UI
{
    public class UserView : MonoBehaviour
    {
        public TMP_Text _text;

        public UserDTO User { get; private set; }
        private bool _online;

        public void Initialize(UserDTO user, bool online)
        {
            _online = online;
            User = user;
            DrawView();
        }

        public void SetOnline(bool value)
        {
            _online = value;
            DrawView();
        }

        private void DrawView()
        {
            UColor color = Color.FromArgb((int) User.Color).ToUColor();
            _text.text = $"<color=#{color.ToHexString()}>{User.Name}</color>" + (_online ? "online" : "");
        }
    }
}