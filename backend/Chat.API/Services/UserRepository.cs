using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.DataAccess;
using Chat.Shared.DTO;
using Mapster;
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

            EntityEntry<User> entry = _context.Users.Add(new User() {Name = nickname, Color = color});
            await _context.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<User?> Get(string nickname, long color)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Name == nickname && user.Color == color);
        }

        public async Task<int> CountUsers() =>
            await _context.Users.CountAsync();

        public async Task<UserDTO[]> GetUsers(int pageNum, int pageSize)
        {
            UserDTO[] dtos = await _context.Users
                .AsNoTracking()
                .OrderBy(user => user.Name)
                .Skip(pageNum * pageSize)
                .Take(pageSize)
                .ProjectToType<UserDTO>()
                .ToArrayAsync();
            return dtos;
        }
    }
}