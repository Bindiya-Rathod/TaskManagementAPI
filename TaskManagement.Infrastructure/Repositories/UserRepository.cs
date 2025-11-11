using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Models;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagementContext _context;

        public UserRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedDate = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
