using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Validators;
using TaskManagement.Shared.Models;

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
                throw new Core.Exceptions.ValidationException("UserId cannot be empty");

            var tasks = await _taskService.GetTasksByUserIdAsync(userId);
            return Ok(tasks);
        }

        [HttpPut("{id}/assign")]
        public async Task<ActionResult<TaskResponse>> AssignTask(int id, [FromBody] AssignTaskRequest request)
        {
            // Validate using the new validator
            AssignTaskRequestValidator.Validate(request);

            _logger.LogInformation("AssignTask called for Task {TaskId}, AssignTo {UserId}",
                                    id, request.AssignedToUserId);

            // Service layer handles all logic + exceptions
            var updatedTask = await _taskService.AssignTaskAsync(id, request.AssignedToUserId);

            return Ok(updatedTask);
        }


        /// <summary>
        /// Updates task status (optional - for better API design)
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorDetail
                    {
                        Code = "INVALID_REQUEST",
                        Message = "Invalid request data"
                    });
                }

                // Use your existing UpdateTaskAsync with partial update
                var updateRequest = new UpdateTaskRequest
                {
                    StatusId = request.StatusId
                };

                var result = await _taskService.UpdateTaskAsync(id, updateRequest);

                if (result == null)
                {
                    return NotFound(new ErrorDetail
                    {
                        Code = "TASK_NOT_FOUND",
                        Message = $"Task with ID {id} not found"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                return StatusCode(500, new ErrorDetail
                {
                    Code = "INTERNAL_ERROR",
                    Message = "An error occurred while processing your request"
                });
            }
        }
        public class UpdateStatusRequest
        {
            [Required]
            [Range(1, 5, ErrorMessage = "StatusId must be between 1 and 5")]
            public int StatusId { get; set; }
        }
    }

}
