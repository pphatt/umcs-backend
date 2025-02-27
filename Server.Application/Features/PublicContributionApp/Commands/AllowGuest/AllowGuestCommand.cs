using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.PublicContributionApp.Commands.AllowGuest;

public class AllowGuestCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public Guid contributionId { get; set; }

    public Guid facultyId { get; set; }
}
