using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Core.Validators
{
    /// <summary>
    /// Validator for UpdateTaskRequest DTO
    /// </summary>
    public static class UpdateTaskRequestValidator
    {
        /// <summary>
        /// Validate update task request
        /// </summary>
        public static void Validate(UpdateTaskRequest request)
        {
            var errors = new List<string>();

            if (request.Title != null && request.Title.Length > 200)
                errors.Add("Title must not exceed 200 characters");

            if (request.StatusId.HasValue && request.StatusId.Value <= 0)
                errors.Add("StatusId must be greater than 0");

            if (request.PriorityId.HasValue && request.PriorityId.Value <= 0)
                errors.Add("PriorityId must be greater than 0");

            if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
                errors.Add("DueDate cannot be in the past");

            if (errors.Any())
                throw new ValidationException("Validation failed for UpdateTaskRequest", errors);
        }
    }
}
