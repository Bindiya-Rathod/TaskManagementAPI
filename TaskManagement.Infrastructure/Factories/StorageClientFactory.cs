using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TaskManagement.Shared.Configuration;

namespace TaskManagement.Infrastructure.Factories
{
    // <summary>
    /// Factory for creating Azure Storage clients
    /// </summary>
    public class StorageClientFactory : IStorageClientFactory
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;

        public StorageClientFactory(
            IConfiguration configuration,
            IOptions<AzureStorageSettings> settings)
        {
            // Try to get from Key Vault first, fallback to settings
            var connectionString = configuration["StorageConnectionString"]
                                ?? settings.Value.ConnectionString;

            _blobServiceClient = new BlobServiceClient(connectionString);
            _queueServiceClient = new QueueServiceClient(connectionString);
        }

        public BlobContainerClient CreateBlobContainerClient(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();
            return containerClient;
        }

        public QueueClient CreateQueueClient(string queueName)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            queueClient.CreateIfNotExists();
            return queueClient;
        }
    }
}
