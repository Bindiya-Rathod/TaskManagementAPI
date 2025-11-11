namespace TaskManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository Tasks { get; }
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
