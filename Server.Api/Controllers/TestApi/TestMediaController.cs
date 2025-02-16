using Microsoft.AspNetCore.Mvc;
using Server.Api.Common.Filters;
using Server.Application.Common.Dtos.Content.Media;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Contracts.Common.Media;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestMediaController : TestApiController
{
    private readonly IMediaService _mediaService;

    public TestMediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("upload-files")]
    [FileValidationFilter(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files, [FromForm] FileRequiredParamsDto request)
    {
        //await _mediaService.SaveFilesAsync(files, type);

        await _mediaService.UploadFilesToCloudinary(files, request);

        return Ok("Save files successfully.");
    }

    [HttpDelete("remove-files")]
    public async Task<IActionResult> RemoveFiles(List<DeleteFilesRequest> request)
    {
        //await _mediaService.RemoveFiles(paths);

        await _mediaService.RemoveFilesFromCloudinary(request);

        return Ok("Delete files successfully.");
    }

    [HttpGet("download-files")]
    public async Task<IActionResult> DownloadFile([FromQuery] List<string> request)
    {
        //try
        //{
        //    var (fileStream, contentType, fileName) = await _mediaService.DownloadFiles(paths);

        //    if (fileStream is MemoryStream memoryStream)
        //    {
        //        return File(memoryStream.ToArray(), contentType, fileName);
        //    }

        //    return File(fileStream, contentType, fileName);
        //}
        //catch (FileNotFoundException ex)
        //{
        //    return NotFound(ex.Message);
        //}
        //catch (Exception ex)
        //{
        //    return StatusCode(500, $"An error occurred: {ex.Message}");
        //}

        var url = _mediaService.GenerateDownloadUrl(request);

        return Ok(url);
    }
}
