using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Factories;
using TaskManagement.Shared.Configuration;
using TaskManagement.Shared.Helpers;

namespace TaskManagement.Infrastructure.Services
{
    /// <summary>
    /// Service for managing Azure Queue operations
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly IStorageClientFactory _storageFactory;
        private readonly AzureStorageSettings _settings;
        private readonly ILogger<QueueService> _logger;

        public QueueService(
            IStorageClientFactory storageFactory,
            IOptions<AzureStorageSettings> settings,
            ILogger<QueueService> logger)
        {
            _storageFactory = storageFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendTaskAssignmentMessageAsync(object message)
        {
            try
            {
                var queueClient = _storageFactory.CreateQueueClient(_settings.TaskAssignmentQueue);
                var messageJson = JsonHelper.Serialize(message);

                _logger.LogInformation("Sending message to queue: {QueueName}", _settings.TaskAssignmentQueue);
                await queueClient.SendMessageAsync(messageJson);

                _logger.LogInformation("Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue");
                throw;
            }
        }
    }
}
