using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class TaskItem
    {
        public int TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string? AssignedToUserId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        public TaskStatus? Status { get; set; }
        public TaskPriority? Priority { get; set; }
        public User? CreatedBy { get; set; }
        public User? AssignedTo { get; set; }
    }
}
