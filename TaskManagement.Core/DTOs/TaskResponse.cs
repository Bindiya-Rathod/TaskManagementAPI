using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs
{
    public class TaskResponse
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string PriorityName { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
