using System;

namespace Chat.Shared.DTO
{
    public class UserDTO : IEquatable<UserDTO>
    {
        public long Color { get; }

        public string Name { get; }

        public UserDTO(string name, long color)
        {
            Color = color;
            Name = name;
        }

        public bool Equals(UserDTO other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Color == other.Color && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserDTO) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Color.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }

        public static bool operator ==(UserDTO left, UserDTO right) => 
            Equals(left, right);

        public static bool operator !=(UserDTO left, UserDTO right) => 
        !Equals(left, right);
    }
}