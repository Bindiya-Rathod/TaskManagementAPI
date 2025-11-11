using Microsoft.EntityFrameworkCore.Storage;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TaskManagementContext _context;
        private IDbContextTransaction? _transaction;

        public ITaskRepository Tasks { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(
            TaskManagementContext context,
            ITaskRepository taskRepository,
            IUserRepository userRepository)
        {
            _context = context;
            Tasks = taskRepository;
            Users = userRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
