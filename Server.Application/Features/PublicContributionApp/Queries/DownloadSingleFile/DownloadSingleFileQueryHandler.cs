using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Queries.DownloadSingleFile;

public class DownloadSingleFileQueryHandler : IRequestHandler<DownloadSingleFileQuery, ErrorOr<ResponseWrapper<string>>>
{
    private readonly IMediaService _mediaService;

    public DownloadSingleFileQueryHandler(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper<string>>> Handle(DownloadSingleFileQuery request, CancellationToken cancellationToken)
    {
        var publicIds = request.PublicIds;

        var url = _mediaService.GenerateDownloadUrl(publicIds);

        return new ResponseWrapper<string>
        {
            IsSuccessful = true,
            Message = "Link download below",
            ResponseData = url
        };
    }
}
