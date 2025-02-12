using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Interfaces.Services.Media;

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
    public async Task<IActionResult> UploadFiles(List<IFormFile> files, string type)
    {
        await _mediaService.SaveFilesAsync(files, type);

        return Ok("Save files successfully.");
    }

    [HttpDelete("remove-files")]
    public async Task<IActionResult> RemoveFiles(List<string> paths)
    {
        await _mediaService.RemoveFiles(paths);

        return Ok("Delete files successfully.");
    }

    [HttpGet("download-files")]
    public async Task<IActionResult> DownloadFile([FromQuery] List<string> paths)
    {
        try
        {
            var (fileStream, contentType, fileName) = await _mediaService.DownloadFiles(paths);

            if (fileStream is MemoryStream memoryStream)
            {
                return File(memoryStream.ToArray(), contentType, fileName);
            }

            return File(fileStream, contentType, fileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
}
