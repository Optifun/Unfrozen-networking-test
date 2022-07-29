namespace Chat.Shared.DTO
{
    public class UserDTO
    {
        public long Color { get; }

        public string Name { get; }

        public UserDTO(long color, string name)
        {
            Color = color;
            Name = name;
        }
    }
}