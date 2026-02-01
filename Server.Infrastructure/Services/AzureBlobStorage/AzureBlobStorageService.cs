using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

using Microsoft.Extensions.Options;

using Server.Application.Common.Interfaces.Services.AzureBlobStorage;
using Server.Infrastructure.Persistence.Repositories;
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

    public string? GetBlobSasUrl(string? blobUrl)
    {
        if (blobUrl is null) return null;

        var blobUri = new Uri(blobUrl);
        var blobClient = new BlobClient(blobUri, new BlobClientOptions());

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _options.ContainerName,
            Resource = "b",
            BlobName = blobClient.Name,
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

        var blobServiceClient = new BlobServiceClient(_options.ConnectionString);

        var sasToken = sasBuilder
            .ToSasQueryParameters(new StorageSharedKeyCredential(blobServiceClient.AccountName, _options.AccountKey))
            .ToString();

        return $"{blobUrl}?{sasToken}";
    }
}
