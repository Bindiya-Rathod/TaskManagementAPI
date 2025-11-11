using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.DTOs
{
    public class UpdateTaskRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? AssignedToUserId { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public DateTime? DueDate { get; set; }
    }

}
