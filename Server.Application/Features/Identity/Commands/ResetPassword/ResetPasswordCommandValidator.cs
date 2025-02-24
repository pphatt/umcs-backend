using FluentValidation;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Server.Application.Features.Identity.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
    }
}
