namespace Server.Infrastructure.Services.Storage;

public class AzureBlobStorageOptions
{
    public string? ConnectionString { get; set; }
    public string? ContainerName { get; set; }
}
