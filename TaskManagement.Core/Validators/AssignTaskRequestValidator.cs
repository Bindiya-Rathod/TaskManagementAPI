using TaskManagement.Core.DTOs;
using TaskManagement.Core.Exceptions;

namespace TaskManagement.Core.Validators
{
    public static class AssignTaskRequestValidator
    {
        public static void Validate(AssignTaskRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                errors.Add("Request body is required.");
                throw new ValidationException("Validation failed for AssignTaskRequest", errors);
            }

            if (string.IsNullOrWhiteSpace(request.AssignedToUserId))
            {
                errors.Add("AssignedToUserId is required.");
            }

            if (errors.Any())
                throw new ValidationException("Validation failed for AssignTaskRequest", errors);
        }
    }
}
