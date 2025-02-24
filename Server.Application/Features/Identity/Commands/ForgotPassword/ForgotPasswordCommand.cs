using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<ErrorOr<ResponseWrapper>>
{
    public string Email { get; set; } = default!;
}
