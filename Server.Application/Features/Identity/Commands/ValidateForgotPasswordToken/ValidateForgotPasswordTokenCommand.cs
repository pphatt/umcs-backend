using ErrorOr;
using MediatR;
using Server.Application.Wrapper;

namespace Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;

public class ValidateForgotPasswordTokenCommand : IRequest<ErrorOr<ResponseWrapper<bool>>>
{
    public string Token { get; set; }
}
