using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace TaskManagement.Infrastructure.Factories
{
    /// <summary>
    /// Factory pattern for creating Azure Storage clients
    /// </summary>
    public interface IStorageClientFactory
    {
        BlobContainerClient CreateBlobContainerClient(string containerName);
        QueueClient CreateQueueClient(string queueName);
    }
}
