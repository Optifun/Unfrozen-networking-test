using System;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Chat.API.Services
{
    public class UserRepository
    {
        private readonly ChatContext _context;

        public UserRepository(ChatContext context) =>
            _context = context;

        public async Task<User> GetOrAdd(string nickname, long color)
        {
            User? user = await Get(nickname, color);
            if (user != null) return user;

            EntityEntry<User> entry = _context.Users.Add(new User() { Name = nickname, Color = color});
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<User?> Get(string nickname, long color)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Name == nickname && user.Color == color);
        }
    }
}