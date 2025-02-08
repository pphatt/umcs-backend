using ErrorOr;
using MediatR;
using Server.Contracts.Authentication.RefreshToken;

namespace Server.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ErrorOr<RefreshTokenResult>>
{
    public string RefreshToken { get; set; } = default!;
}
