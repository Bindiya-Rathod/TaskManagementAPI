using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class TaskHistory
    {
        public int HistoryId { get; set; }
        public int TaskId { get; set; }
        public string ChangedByUserId { get; set; } = string.Empty;
        public string ChangeType { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedDate { get; set; } = DateTime.UtcNow;
    }
}
