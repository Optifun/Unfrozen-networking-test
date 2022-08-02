using System.Collections.Generic;
using System.Linq;
using Chat.Shared.DTO;

namespace Chat.API.Services
{
    public class InMemoryChatUsers : IChatUsers
    {
        private readonly Dictionary<string, UserDTO> _users = new();

        public List<UserDTO> GetUsers() =>
            _users.Values.ToList();

        public UserDTO? Get(string connectionId)
        {
            if (_users.TryGetValue(connectionId, out var value))
                return value;
            return null;
        }

        public void Add(string connectionId, UserDTO user)
        {
            _users.Add(connectionId, user);
        }

        public void Remove(UserDTO user)
        {
            var pair = _users.FirstOrDefault(pair => pair.Value == user);
            Remove(pair.Key);
        }

        public void Remove(string connectionId) =>
            _users.Remove(connectionId);
    }
}