using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionTagApp.Commands.CreateTag;

public class CreateTagCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string TagName { get; set; }
}
