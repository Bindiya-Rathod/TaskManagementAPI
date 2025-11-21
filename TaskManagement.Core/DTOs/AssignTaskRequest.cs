using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.DTOs
{
    /// <summary>
    /// Request model for assigning a task to a user
    /// </summary>
    public class AssignTaskRequest
    {
        /// <summary>
        /// The user ID to assign the task to
        /// </summary>
        [Required(ErrorMessage = "AssignedToUserId is required")]
        public string AssignedToUserId { get; set; } = string.Empty;
    }
}
