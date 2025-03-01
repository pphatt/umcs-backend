using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Queries.DownloadAllFiles;

public class DownloadAllFilesQuery : IRequest<ErrorOr<ResponseWrapper<string>>>
{
    public Guid ContributionId { get; set; }
}
