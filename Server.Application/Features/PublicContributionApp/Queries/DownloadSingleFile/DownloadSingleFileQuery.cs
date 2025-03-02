using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Queries.DownloadSingleFile;

public class DownloadSingleFileQuery : IRequest<ErrorOr<ResponseWrapper<string>>>
{
    public List<string> PublicIds { get; set; }
}
