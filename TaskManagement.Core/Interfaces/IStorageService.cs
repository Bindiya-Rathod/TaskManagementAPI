namespace TaskManagement.Core.Interfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName);
        Task<Stream> DownloadFileAsync(string blobUrl);
        Task<bool> DeleteFileAsync(string blobUrl);
    }
}
