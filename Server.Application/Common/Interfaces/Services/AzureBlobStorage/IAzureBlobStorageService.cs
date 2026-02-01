namespace Server.Application.Common.Interfaces.Services.AzureBlobStorage;

public interface IAzureBlobStorageService
{
    public Task<string> UploadFileToBlobStorage(Stream file, string fileName);
}
