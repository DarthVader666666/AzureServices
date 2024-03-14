using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.Reflection.Metadata;

namespace Mvc.StorageAccount.Demo.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IConfiguration _configuration;
        private string containerName = "attendeeimages";

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadBlob(IFormFile formfile, string imageName, string? originalBlobName = null)
        {
            var blobName = $"{imageName}{Path.GetExtension(formfile.FileName)}";
            var container = await GetBlobContainerClient();

            if (!string.IsNullOrEmpty(originalBlobName))
            {
                await RemoveBlob(originalBlobName);
            }

            using var memoryStream = new MemoryStream();
            formfile.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(memoryStream, overwrite: true);
            return blobName;
        }

        public async Task<string> GetBlobUrl(string imageName)
        {
            var container = await GetBlobContainerClient();
            var blob = container.GetBlobClient(imageName);

            BlobSasBuilder blobSasBuilder = new()
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),
                Protocol = SasProtocol.Https,
                Resource = "b"
            };

            blobSasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

            return blob.GenerateSasUri(blobSasBuilder).ToString();
        }

        public async Task RemoveBlob(string imageName)
        {
            var container = await GetBlobContainerClient();
            var blob = container.GetBlobClient(imageName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }

        private async Task<BlobContainerClient> GetBlobContainerClient()
        {
            try
            {
                var container = new BlobContainerClient(_configuration["AzureStorage:ConnectionString"], containerName);
                await container.CreateIfNotExistsAsync();
                return container;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
