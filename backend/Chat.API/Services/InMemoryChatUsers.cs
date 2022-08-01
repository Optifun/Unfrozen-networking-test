using System.Collections.Generic;
using Chat.Shared.DTO;

namespace Chat.API.Services
{
    public class InMemoryChatUsers : IChatUsers
    {
        private readonly List<UserDTO> _users = new List<UserDTO>();

        public List<UserDTO> GetUsers() =>
            _users;

        public void Add(UserDTO user) =>
            _users.Add(user);

        public void Remove(UserDTO user) =>
            _users.Remove(user);
    }
}