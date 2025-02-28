using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;

public class AllowGuestWithManyContributionsCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public List<Guid> ContributionIds { get; set; } = default!;

    public Guid FacultyId { get; set; }
}
