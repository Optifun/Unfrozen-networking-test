using System.Drawing;

namespace Chat.API.DTO
{
    public record MessageDTO(string Username, string Text, Color Color);
}