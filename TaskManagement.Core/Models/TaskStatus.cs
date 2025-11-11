using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class TaskStatus
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
