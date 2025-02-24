using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string Token { get; set; } = default!;

    public string Password { get; set; } = default!;
}
