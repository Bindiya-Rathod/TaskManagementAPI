using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Shared.Models
{
    /// <summary>
    /// Standardized error response model for API responses
    /// </summary>
    public class ErrorResponse
    {
        public ErrorDetail Error { get; set; } = new ErrorDetail();
    }

    /// <summary>
    /// Error detail information
    /// </summary>
    public class ErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> Details { get; set; } = new List<string>();
    }
}
