using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Factories;
using TaskManagement.Shared.Configuration;

namespace TaskManagement.Infrastructure.Services
{
    /// <summary>
    /// Service for managing Azure Blob Storage operations
    /// </summary>
    public class StorageService : IStorageService
    {
        private readonly IStorageClientFactory _storageFactory;
        private readonly AzureStorageSettings _settings;
        private readonly ILogger<StorageService> _logger;

        public StorageService(
            IStorageClientFactory storageFactory,
            IOptions<AzureStorageSettings> settings,
            ILogger<StorageService> logger)
        {
            _storageFactory = storageFactory;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName)
        {
            try
            {
                var containerClient = _storageFactory.CreateBlobContainerClient(containerName);
                var blobName = $"{Guid.NewGuid()}_{fileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                _logger.LogInformation("Uploading file {FileName} to {Container}", fileName, containerName);

                await blobClient.UploadAsync(fileStream, overwrite: true);

                _logger.LogInformation("Upload successful: {BlobUrl}", blobClient.Uri);
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string blobUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(blobUrl));
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from {BlobUrl}", blobUrl);
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string blobUrl)
        {
            try
            {
                var blobClient = new BlobClient(new Uri(blobUrl));
                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file from {BlobUrl}", blobUrl);
                return false;
            }
        }
    }
}
