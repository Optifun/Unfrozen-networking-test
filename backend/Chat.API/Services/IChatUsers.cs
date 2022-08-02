using System.Collections.Generic;
using Chat.Shared.DTO;

namespace Chat.API.Services
{
    public interface IChatUsers
    {
        public List<UserDTO> GetUsers();
        public UserDTO? Get(string connectionId);
        void Add(string connectionId, UserDTO user);
        void Remove(UserDTO user);
        void Remove(string connectionId);
    }
}