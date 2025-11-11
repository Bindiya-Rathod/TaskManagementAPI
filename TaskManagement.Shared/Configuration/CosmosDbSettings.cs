namespace TaskManagement.Shared.Configuration
{
    // <summary>
    /// CosmosDB configuration settings using Options Pattern
    /// </summary>
    public class CosmosDbSettings
    {
        public const string SectionName = "CosmosDb";

        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = "TaskManagementDB";
        public string TaskDocumentsContainer { get; set; } = "TaskDocuments";
        public string TaskAnalyticsContainer { get; set; } = "TaskAnalytics";
    }
}
