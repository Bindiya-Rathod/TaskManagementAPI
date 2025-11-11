using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Models;
using TaskManagement.Infrastructure.Data;
namespace TaskManagement.API.Controllers
{
    // <summary>
    /// Attachments controller
    /// </summary>
    [ApiController]
    [Route("api/tasks/{taskId}/[controller]")]
    public class AttachmentsController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IDocumentRepository _documentRepository;
        private readonly TaskManagementContext _context;

        public AttachmentsController(
            IStorageService storageService,
            IDocumentRepository documentRepository,
            TaskManagementContext context)
        {
            _storageService = storageService;
            _documentRepository = documentRepository;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "File is required" });

            var taskExists = await _context.Tasks.AnyAsync(t => t.TaskId == taskId);
            if (!taskExists) return NotFound(new { error = "Task not found" });

            using var stream = file.OpenReadStream();
            var blobUrl = await _storageService.UploadFileAsync(stream, file.FileName, "task-attachments");

            var cosmosDoc = new
            {
                id = Guid.NewGuid().ToString(),
                taskId = taskId.ToString(),
                fileName = file.FileName,
                blobUrl = blobUrl,
                uploadedDate = DateTime.UtcNow,
                metadata = new { fileSize = file.Length, contentType = file.ContentType }
            };

            await _documentRepository.SaveDocumentMetadataAsync(cosmosDoc);

            var attachment = new TaskAttachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                BlobUrl = blobUrl,
                CosmosDocId = cosmosDoc.id,
                FileSizeBytes = file.Length
            };

            _context.TaskAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(new { attachmentId = attachment.AttachmentId, fileName = file.FileName, blobUrl });
        }

        [HttpGet]
        public async Task<ActionResult> GetAttachments(int taskId)
        {
            var attachments = await _context.TaskAttachments.Where(a => a.TaskId == taskId).ToListAsync();
            return Ok(attachments);
        }
    }

}
