using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Options;
using TaskManagement.Shared.Configuration;

namespace TaskManagement.Infrastructure.Factories
{
    // <summary>
    /// Factory for creating Azure Storage clients
    /// </summary>
    public class StorageClientFactory : IStorageClientFactory
    {
        private readonly AzureStorageSettings _settings;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly QueueServiceClient _queueServiceClient;

        public StorageClientFactory(IOptions<AzureStorageSettings> settings)
        {
            _settings = settings.Value;
            _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            _queueServiceClient = new QueueServiceClient(_settings.ConnectionString);
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
