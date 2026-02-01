using Azure.Storage.Blobs;

using Microsoft.Extensions.Options;

using Server.Application.Common.Interfaces.Services.AzureBlobStorage;
using Server.Infrastructure.Services.Storage;

namespace Server.Infrastructure.Services.AzureBlobStorage;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly AzureBlobStorageOptions _options;

    public AzureBlobStorageService(IOptions<AzureBlobStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> UploadFileToBlobStorage(Stream file, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(file);

        var blobUrl = blobClient.Uri.ToString();
        return blobUrl;
    }
}
