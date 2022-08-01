using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Chat.API.Services
{
    public class MessageRepository
    {
        private readonly ChatContext _context;
        private readonly UserRepository _userRepository;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ChatContext context, UserRepository userRepository, ILogger<MessageRepository> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
            _context = context;
        }

        public async Task<List<Message>> GetLast(int count)
        {
            List<Message> list = await _context.Messages
                .Include(msg => msg.User)
                .OrderByDescending(msg => msg.Created)
                .Take(count)
                .AsSingleQuery()
                .ToListAsync();
            return list;
        }

        public async Task Create(string username, string text, long color)
        {
            User? user = await _userRepository.Get(username, color);
            if (user == null)
                throw new InvalidOperationException($"User {username}:{color} dont exists");

            _logger.Log(LogLevel.Debug, $"User found user with {user.Messages.Count} messages");
            EntityEntry<Message> entry = _context.Messages.Add(new Message() {User = user, Text = text});
            await _context.SaveChangesAsync();
            _logger.Log(LogLevel.Debug,$"Created message {entry.Entity}");
        }
    }
}