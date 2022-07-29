using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class MessageBubble : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Initialize(string username, Color color, string text)
        {
            _text.text = $"<color={color.ToHexString()}>{username}</color>: {text}";
        }
    }
}