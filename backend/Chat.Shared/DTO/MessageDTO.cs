namespace Chat.Shared.DTO
{
    public class MessageDTO
    {
        public MessageDTO(string username, string text, long color)
        {
            this.Username = username;
            this.Text = text;
            this.Color = color;
        }

        public string Username { get; }
        public string Text { get; }
        public long Color { get; }

        public void Deconstruct(out string Username, out string Text, out long Color)
        {
            Username = this.Username;
            Text = this.Text;
            Color = this.Color;
        }
    }
}