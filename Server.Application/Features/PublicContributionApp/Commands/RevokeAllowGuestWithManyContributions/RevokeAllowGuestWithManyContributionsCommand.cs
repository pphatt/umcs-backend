using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuestWithManyContributions;

public class RevokeAllowGuestWithManyContributionsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> ContributionIds { get; set; } = default!;

    public Guid UserId { get; set; }

    public Guid UserFacultyId { get; set; }
}
