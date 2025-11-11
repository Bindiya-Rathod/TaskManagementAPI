using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Models;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagementContext _context;

        public TaskRepository(TaskManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            return await _context.Tasks
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserIdAsync(string userId)
        {
            return await _context.Tasks
                .Include(t => t.Status)
                .Include(t => t.Priority)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTo)
                .Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId)
                .ToListAsync();
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            task.CreatedDate = DateTime.UtcNow;
            task.ModifiedDate = DateTime.UtcNow;

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int taskId, TaskItem task)
        {
            var existing = await _context.Tasks.FindAsync(taskId);
            if (existing == null) return null;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.AssignedToUserId = task.AssignedToUserId;
            existing.StatusId = task.StatusId;
            existing.PriorityId = task.PriorityId;
            existing.DueDate = task.DueDate;
            existing.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
