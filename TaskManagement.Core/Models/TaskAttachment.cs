using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class TaskAttachment
    {
        public int AttachmentId { get; set; }
        public int TaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
        public string? CosmosDocId { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    }
}
