using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.CreateGuest;

public class CreateGuestCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string Email { get; set; } = default!;

    public string Username { get; set; } = default!;

    public Guid FacultyId { get; set; }
}
