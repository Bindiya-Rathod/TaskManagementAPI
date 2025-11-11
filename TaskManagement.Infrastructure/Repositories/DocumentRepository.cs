using Microsoft.Azure.Cosmos;
using TaskManagement.Core.Interfaces;
using Microsoft.Extensions.Options;
using TaskManagement.Shared.Configuration;

namespace TaskManagement.Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly Container _container;

        public DocumentRepository(CosmosClient cosmosClient, IOptions<CosmosDbSettings> settings)
        {
            var config = settings.Value;
            _container = cosmosClient.GetContainer(config.DatabaseName, config.TaskDocumentsContainer);
        }

        public async Task<string> SaveDocumentMetadataAsync(object document)
        {
            var response = await _container.CreateItemAsync(document);
            return response.Resource.ToString() ?? string.Empty;
        }

        public async Task<T?> GetDocumentByIdAsync<T>(string id, string partitionKey)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }
    }
}
