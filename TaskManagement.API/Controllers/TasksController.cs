using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Validators;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Tasks controller for managing task operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAllTasks()
        {
            var correlationId = HttpContext.TraceIdentifier;
            _logger.LogInformation("GetAllTasks - CorrelationId: {CorrelationId}", correlationId);

            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponse>> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null) throw new TaskNotFoundException(id);
            return Ok(task);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] CreateTaskRequest request)
        {
            CreateTaskRequestValidator.Validate(request);
            var task = await _taskService.CreateTaskAsync(request);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.TaskId }, task);
        }

        /// <summary>
        /// Update a task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponse>> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
        {
            var task = await _taskService.UpdateTaskAsync(id, request);
            if (task == null) throw new TaskNotFoundException(id);
            return Ok(task);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var deleted = await _taskService.DeleteTaskAsync(id);
            if (!deleted) throw new TaskNotFoundException(id);
            return NoContent();
        }

        /// <summary>
        /// Get tasks by user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TaskResponse>>> GetTasksByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ValidationException("UserId cannot be empty");

            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }
    }

}
