using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string? AssignedToUserId { get; set; }
        public int StatusId { get; set; } = 1;
        public int PriorityId { get; set; } = 2;
        public DateTime? DueDate { get; set; }
    }
}
