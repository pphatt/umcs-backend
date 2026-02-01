using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Interfaces.Services.AzureBlobStorage;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestFileUploadController : TestApiController
{
    private readonly IAzureBlobStorageService _azureBlobStorageService;

    public TestFileUploadController(IAzureBlobStorageService azureBlobStorageService)
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
}
