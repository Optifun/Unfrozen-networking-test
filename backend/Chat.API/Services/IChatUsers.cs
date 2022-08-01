using System.Collections.Generic;
using Chat.Shared.DTO;

namespace Chat.API.Services
{
    public interface IChatUsers
    {
        public List<UserDTO> GetUsers();
        void Add(UserDTO user);
        void Remove(UserDTO user);
    }
}