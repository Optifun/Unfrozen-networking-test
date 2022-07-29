namespace Chat.Shared.DTO
{
    public class UserDTO
    {
        public long Color { get; }

        public string Name { get; }

        public UserDTO(string name, long color)
        {
            Color = color;
            Name = name;
        }
    }
}