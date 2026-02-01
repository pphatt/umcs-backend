using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Interfaces.Services.AzureBlobStorage;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestAzureBlobStorageController : TestApiController
{
    private readonly IAzureBlobStorageService _azureBlobStorageService;

    public TestAzureBlobStorageController(IAzureBlobStorageService azureBlobStorageService)
    {
        _azureBlobStorageService = azureBlobStorageService;
    }

    [HttpPost("azure-upload-file")]
    public async Task<IActionResult> UploadAzureBlobStorage(IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var blobUrl = await _azureBlobStorageService.UploadFileToBlobStorage(stream, file.FileName);

        return Ok(blobUrl);
    }

    [HttpPost("generate-sas-url")]
    public async Task<IActionResult> GetGeneratedSasURL(string? blobUrl)
    {
        var blobSasUrl = _azureBlobStorageService.GetBlobSasUrl(blobUrl);
        return Ok(blobSasUrl);
    }
}
