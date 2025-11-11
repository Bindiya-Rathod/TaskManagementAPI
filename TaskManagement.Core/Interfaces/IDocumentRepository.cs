namespace TaskManagement.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<string> SaveDocumentMetadataAsync(object document);
        Task<T?> GetDocumentByIdAsync<T>(string id, string partitionKey);
    }
}
