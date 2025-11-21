using Microsoft.Extensions.Logging;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Models;

namespace TaskManagement.Infrastructure.Services
{
    /// <summary>
    /// Business logic service for task operations
    /// Implements ITaskService interface from Core
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IQueueService _queueService;
        private readonly ILogger<TaskService> _logger;

        /// <summary>
        /// Constructor with dependency injection
        /// </summary>
        public TaskService(
            IUnitOfWork unitOfWork,
            IQueueService queueService,
            ILogger<TaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _queueService = queueService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        public async Task<IEnumerable<TaskResponse>> GetAllTasksAsync()
        {
            _logger.LogInformation("Getting all tasks");

            var tasks = await _unitOfWork.Tasks.GetAllTasksAsync();

            _logger.LogInformation("Retrieved {Count} tasks", tasks.Count());

            return tasks.Select(MapToResponse);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        public async Task<TaskResponse?> GetTaskByIdAsync(int taskId)
        {
            _logger.LogInformation("Getting task with ID: {TaskId}", taskId);

            var task = await _unitOfWork.Tasks.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", taskId);
                return null;
            }

            return MapToResponse(task);
        }

        /// <summary>
        /// Get tasks by user ID
        /// </summary>
        public async Task<IEnumerable<TaskResponse>> GetTasksByUserIdAsync(string userId)
        {
            _logger.LogInformation("Getting tasks for user: {UserId}", userId);

            var tasks = await _unitOfWork.Tasks.GetTasksByUserIdAsync(userId);

            _logger.LogInformation("Retrieved {Count} tasks for user {UserId}", tasks.Count(), userId);

            return tasks.Select(MapToResponse);
        }

        /// <summary>
        /// Create a new task with transaction support
        /// </summary>
        public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request)
        {
            _logger.LogInformation("Creating new task: {Title}", request.Title);

            // Begin transaction for data consistency
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Create task entity
                var task = new TaskItem
                {
                    Title = request.Title,
                    Description = request.Description,
                    CreatedByUserId = request.CreatedByUserId,
                    AssignedToUserId = request.AssignedToUserId,
                    StatusId = request.StatusId,
                    PriorityId = request.PriorityId,
                    DueDate = request.DueDate
                };

                // Save to database
                var created = await _unitOfWork.Tasks.CreateTaskAsync(task);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Task created with ID: {TaskId}", created.TaskId);

                // Send queue message if task is assigned to someone
                if (!string.IsNullOrEmpty(created.AssignedToUserId))
                {
                    _logger.LogInformation("Sending task assignment message to queue");

                    await _queueService.SendTaskAssignmentMessageAsync(new
                    {
                        TaskId = created.TaskId,
                        AssignedTo = created.AssignedToUserId,
                        Action = "TaskAssigned",
                        Timestamp = DateTime.UtcNow
                    });
                }

                // Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Task creation completed successfully");

                // Return full task details with navigation properties
                var result = await _unitOfWork.Tasks.GetTaskByIdAsync(created.TaskId);
                return MapToResponse(result!);
            }
            catch (Exception ex)
            {
                // Rollback transaction on error
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        public async Task<TaskResponse?> UpdateTaskAsync(int taskId, UpdateTaskRequest request)
        {
            _logger.LogInformation("Updating task: {TaskId}", taskId);

            // Get existing task
            var existing = await _unitOfWork.Tasks.GetTaskByIdAsync(taskId);
            if (existing == null)
            {
                _logger.LogWarning("Task not found for update: {TaskId}", taskId);
                return null;
            }

            // Track old status for queue message
            var oldStatusId = existing.StatusId;

            // Create updated task entity
            var updated = new TaskItem
            {
                TaskId = taskId,
                Title = request.Title ?? existing.Title,
                Description = request.Description ?? existing.Description,
                CreatedByUserId = existing.CreatedByUserId,
                AssignedToUserId = request.AssignedToUserId ?? existing.AssignedToUserId,
                StatusId = request.StatusId ?? existing.StatusId,
                PriorityId = request.PriorityId ?? existing.PriorityId,
                DueDate = request.DueDate ?? existing.DueDate
            };

            // Update in database
            var result = await _unitOfWork.Tasks.UpdateTaskAsync(taskId, updated);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Task updated successfully: {TaskId}", taskId);

            // Send queue message if status changed
            if (request.StatusId.HasValue && request.StatusId.Value != oldStatusId)
            {
                _logger.LogInformation("Status changed, sending queue message");

                await _queueService.SendTaskAssignmentMessageAsync(new
                {
                    TaskId = taskId,
                    Action = "StatusChanged",
                    OldStatus = oldStatusId,
                    NewStatus = request.StatusId.Value,
                    Timestamp = DateTime.UtcNow
                });
            }

            return result == null ? null : MapToResponse(result);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        public async Task<bool> DeleteTaskAsync(int taskId)
        {
            _logger.LogInformation("Deleting task: {TaskId}", taskId);

            var result = await _unitOfWork.Tasks.DeleteTaskAsync(taskId);

            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Task deleted successfully: {TaskId}", taskId);
            }
            else
            {
                _logger.LogWarning("Task not found for deletion: {TaskId}", taskId);
            }

            return result;
        }

        /// <summary>
        /// Map TaskItem entity to TaskResponse DTO
        /// </summary>
        private static TaskResponse MapToResponse(TaskItem task)
        {
            return new TaskResponse
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description,
                StatusName = task.Status?.StatusName ?? "Unknown",
                PriorityName = task.Priority?.PriorityName ?? "Unknown",
                CreatedByName = task.CreatedBy?.FullName ?? "Unknown",
                AssignedToName = task.AssignedTo?.FullName,
                DueDate = task.DueDate,
                CreatedDate = task.CreatedDate
            };
        }
        /// <summary>
        /// Assigns a task to a user with automatic status transition
        /// </summary>
        public async Task<TaskResponse> AssignTaskAsync(int taskId, string assignedToUserId)
        {
            _logger.LogInformation("Assigning task {TaskId} to user {UserId}", taskId, assignedToUserId);

            try
            {
                var task = await _unitOfWork.Tasks.GetTaskByIdAsync(taskId);
                if (task == null)
                    throw new TaskNotFoundException($"Task with ID {taskId} not found");

                var oldAssignee = task.AssignedToUserId;
                task.AssignedToUserId = assignedToUserId;

                if (task.StatusId == 1)
                    task.StatusId = 2; // New → In Progress

                await _unitOfWork.Tasks.UpdateTaskAsync(taskId, task);
                await _unitOfWork.SaveChangesAsync();

                await _queueService.SendTaskAssignmentMessageAsync(new
                {
                    TaskId = taskId,
                    AssignedTo = assignedToUserId,
                    PreviousAssignee = oldAssignee,
                    Action = "Assigned",
                    NewStatus = task.StatusId,
                    Timestamp = DateTime.UtcNow
                });

                var result = await _unitOfWork.Tasks.GetTaskByIdAsync(taskId);
                return MapToResponse(result!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task");
                throw;
            }
        }
    }
}
