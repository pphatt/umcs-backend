using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuest;

public class RevokeAllowGuestCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid ContributionId { get; set; }

    public Guid UserId { get; set; }

    public Guid UserFacultyId { get; set; }
}
