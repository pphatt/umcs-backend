using ErrorOr;
using MediatR;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Common.Interfaces.Services.Media;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;

namespace Server.Application.Features.PublicContributionApp.Queries.DownloadAllFiles;

public class DownloadAllFilesQueryHandler : IRequestHandler<DownloadAllFilesQuery, ErrorOr<ResponseWrapper<string>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;

    public DownloadAllFilesQueryHandler(IUnitOfWork unitOfWork, IMediaService mediaService)
    {
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
    }

    public async Task<ErrorOr<ResponseWrapper<string>>> Handle(DownloadAllFilesQuery request, CancellationToken cancellationToken)
    {
        var contribution = await _unitOfWork.ContributionPublicRepository.GetByIdAsync(request.ContributionId);

        if (contribution is null)
        {
            return Errors.Contribution.NotPublicYet;
        }

        var paths = await _unitOfWork.FileRepository.GetFilesPathByContributionId(contribution.Id);

        if (paths.Count() == 0 || paths.Contains(""))
        {
            return Errors.Contribution.NoFilesFound;
        }

        var result = _mediaService.GenerateDownloadUrl(paths);

        return new ResponseWrapper<string>
        {
            IsSuccessful = true,
            Message = "Link to download below",
            ResponseData = result
        };
    }
}
