using ErrorOr;
using MediatR;
using Server.Application.Wrapper;
using Server.Contracts.Authentication.RefreshToken;

namespace Server.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ErrorOr<ResponseWrapper<RefreshTokenResult>>>
{
    public string RefreshToken { get; set; } = default!;
}
