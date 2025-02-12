using Microsoft.AspNetCore.Http;
using Server.Application.Common.Dtos.Content.Media;

namespace Server.Application.Common.Interfaces.Services.Media;

public interface IMediaService
{
    Task SaveFilesAsync(List<IFormFile> files, string type);

    Task RemoveFiles(List<string> paths);

    Task<(Stream FileStream, string ContentType, string FileName)> DownloadFiles(List<string> path);

    Task<List<FileDto>> UploadFilesToCloudinary(List<IFormFile> files, FileRequiredParamsDto dto);
}
