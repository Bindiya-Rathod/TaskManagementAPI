using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace TaskManagement.Functions.Functions
{
    public class TaskProcessorFunction
    {
        private readonly ILogger<TaskProcessorFunction> _logger;
        private readonly HttpClient _httpClient;
        private readonly CosmosClient _cosmosClient;

        private readonly string _databaseName = "TaskManagementDB";
        private readonly string _analyticsContainer = "TaskAnalytics";
        private readonly string _notificationUrl;

        public TaskProcessorFunction(
            ILogger<TaskProcessorFunction> logger,
            IHttpClientFactory httpClientFactory,
            CosmosClient cosmosClient)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _cosmosClient = cosmosClient;

            // This will come from local.settings.json or App Settings
            _notificationUrl = Environment.GetEnvironmentVariable("NotificationFunctionUrl");
        }

        [Function("TaskProcessorFunction")]
        public async Task Run(
            [QueueTrigger("task-assignment-queue", Connection = "AzureWebJobsStorage")] string queueMessage)
        {
            _logger.LogInformation("TaskProcessorFunction triggered with message: {Message}", queueMessage);

            QueueMessageDto message;
            try
            {
                message = JsonSerializer.Deserialize<QueueMessageDto>(queueMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize queue message");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("Queue message was null after deserialization");
                return;
            }

            // Case 1: Task was Assigned → call Notification Function
            if (message.Action == "TaskAssigned")
            {
                await HandleTaskAssignmentAsync(message);
            }

            // Case 2: Task Status Changed → send simple analytics to CosmosDB
            if (message.Action == "StatusChanged")
            {
                await HandleStatusChangedAsync(message);
            }

            _logger.LogInformation("TaskProcessorFunction completed processing.");
        }

        // -----------------------------------
        // HANDLE TASK ASSIGNMENT
        // -----------------------------------
        private async Task HandleTaskAssignmentAsync(QueueMessageDto message)
        {
            try
            {
                var notify = new NotificationRequestDto
                {
                    TaskId = message.TaskId,
                    UserId = message.AssignedTo!,
                    Message = "A new task has been assigned to you."
                };

                var json = JsonSerializer.Serialize(notify);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Calling NotificationFunction: {Url}", _notificationUrl);

                var response = await _httpClient.PostAsync(_notificationUrl, content);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Notification sent successfully.");
                else
                    _logger.LogWarning("Notification Function returned: {Status}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling NotificationFunction.");
            }
        }

        // -----------------------------------
        // HANDLE STATUS CHANGED → COSMOS ANALYTICS
        // -----------------------------------
        private async Task HandleStatusChangedAsync(QueueMessageDto message)
        {
            try
            {
                var container = _cosmosClient.GetContainer(_databaseName, _analyticsContainer);

                var doc = new AnalyticsEvent
                {
                    id = Guid.NewGuid().ToString(),
                    date = message.Timestamp.ToString("yyyy-MM-dd"),
                    taskId = message.TaskId,
                    action = message.Action,
                    timestamp = message.Timestamp,
                    oldStatus = message.OldStatus,
                    newStatus = message.NewStatus
                };

                await container.CreateItemAsync(doc, new PartitionKey(doc.date));

                _logger.LogInformation("Analytics event written to CosmosDB.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing analytics event to CosmosDB.");
            }
        }
    }

    // -----------------------------------
    // DTOs for Queue Messages & Analytics
    // -----------------------------------

    public class QueueMessageDto
    {
        public int TaskId { get; set; }
        public string Action { get; set; }
        public string? AssignedTo { get; set; }
        public int? OldStatus { get; set; }
        public int? NewStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class NotificationRequestDto
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
    }

    public class AnalyticsEvent
    {
        public string id { get; set; }
        public string date { get; set; }
        public int taskId { get; set; }
        public string action { get; set; }
        public DateTime timestamp { get; set; }
        public int? oldStatus { get; set; }
        public int? newStatus { get; set; }
    }
}
