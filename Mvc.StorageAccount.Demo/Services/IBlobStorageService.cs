
namespace Mvc.StorageAccount.Demo.Services
{
    public interface IBlobStorageService
    {
        Task<string> GetBlobUrl(string imageName);
        Task RemoveBlob(string imageName);
        Task<string> UploadBlob(IFormFile formfile, string imageName, string? originalBlobName = null);
    }
}