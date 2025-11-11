using TaskManagement.Core.DTOs;

namespace TaskManagement.Core.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponse>> GetAllTasksAsync();
        Task<TaskResponse?> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(string userId);
        Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request);
        Task<TaskResponse?> UpdateTaskAsync(int taskId, UpdateTaskRequest request);
        Task<bool> DeleteTaskAsync(int taskId);
    }
}
