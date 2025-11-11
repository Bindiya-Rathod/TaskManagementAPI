using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Core.Validators
{
    /// <summary>
    /// Validator for CreateTaskRequest DTO
    /// Implements input validation as per coding standards
    /// </summary>
    public static class CreateTaskRequestValidator
    {
        /// <summary>
        /// Validate create task request and throw ValidationException if invalid
        /// </summary>
        /// <param name="request">The request to validate</param>
        /// <exception cref="ValidationException">Thrown when validation fails</exception>
        public static void Validate(CreateTaskRequest request)
        {
            var errors = new List<string>();

            // Title validation
            if (string.IsNullOrWhiteSpace(request.Title))
                errors.Add("Title is required");

            if (request.Title?.Length > 200)
                errors.Add("Title must not exceed 200 characters");

            // User ID validation
            if (string.IsNullOrWhiteSpace(request.CreatedByUserId))
                errors.Add("CreatedByUserId is required");

            // Status validation
            if (request.StatusId <= 0)
                errors.Add("StatusId must be greater than 0");

            // Priority validation
            if (request.PriorityId <= 0)
                errors.Add("PriorityId must be greater than 0");

            // Due date validation
            if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
                errors.Add("DueDate cannot be in the past");

            if (errors.Any())
                throw new ValidationException("Validation failed for CreateTaskRequest", errors);
        }
    }
}
