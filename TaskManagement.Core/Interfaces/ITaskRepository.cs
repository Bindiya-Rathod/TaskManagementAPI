using TaskManagement.Core.Models;

namespace TaskManagement.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId);
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(int taskId, TaskItem task);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}
