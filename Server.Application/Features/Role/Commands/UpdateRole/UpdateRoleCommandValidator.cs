using FluentValidation;

namespace Server.Application.Features.Role.Commands.UpdateRole;

public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(r => r.DisplayName)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();

        RuleFor(r => r.Name)
            .MaximumLength(256)
            .NotEmpty()
            .NotNull();
    }
}
