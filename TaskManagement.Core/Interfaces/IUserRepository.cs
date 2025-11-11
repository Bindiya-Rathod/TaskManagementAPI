using TaskManagement.Core.Models;

namespace TaskManagement.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string userId);
        Task<User> CreateUserAsync(User user);
    }
}
