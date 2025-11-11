using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class TaskPriority
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
