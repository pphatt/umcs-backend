using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionCommentApp.Commands;

public class CreatePublicCommentCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid ContributionId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; }
}
