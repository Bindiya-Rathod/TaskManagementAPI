namespace TaskManagement.Shared.Configuration
{
    /// <summary>
    /// Azure Storage configuration settings using Options Pattern
    /// </summary>
    public class AzureStorageSettings
    {
        public const string SectionName = "AzureStorage";

        public string ConnectionString { get; set; } = string.Empty;
        public string TaskAttachmentsContainer { get; set; } = "task-attachments";
        public string TaskReportsContainer { get; set; } = "task-reports";
        public string TaskAssignmentQueue { get; set; } = "task-assignment-queue";
    }
}
