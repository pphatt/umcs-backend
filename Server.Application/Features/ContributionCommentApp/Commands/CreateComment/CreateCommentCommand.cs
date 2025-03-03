using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.ContributionCommentApp.Commands.CreateComment;

public class CreateCommentCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid ContributionId { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = default!;
}
