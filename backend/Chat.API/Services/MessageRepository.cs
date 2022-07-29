using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Services
{
    public class MessageRepository
    {
        private readonly ChatContext _context;
        private readonly UserRepository _userRepository;

        public MessageRepository(ChatContext context, UserRepository userRepository)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetLast(int count)
        {
            return await _context.Messages
                .OrderBy(msg => msg.Created)
                .TakeLast(count)
                .ToArrayAsync();
        }

        public async Task Create(string username, string text, long color)
        {
            User? user = await _userRepository.Get(username, color);
            if (user == null)
                throw new InvalidOperationException($"User {username}:{color} dont exists");

            _context.Messages.Add(new Message() {Id = Guid.NewGuid(), User = user, Text = text});
        }
    }
}