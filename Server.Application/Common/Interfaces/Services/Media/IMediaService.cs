using Microsoft.AspNetCore.Http;

namespace Server.Application.Common.Interfaces.Services.Media;

public interface IMediaService
{
    Task SaveFilesAsync(List<IFormFile> files, string type);

    Task RemoveFiles(List<string> paths);

    Task<(Stream FileStream, string ContentType, string FileName)> DownloadFiles(List<string> path);
}
