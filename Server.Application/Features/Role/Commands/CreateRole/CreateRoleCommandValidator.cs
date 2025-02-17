using FluentValidation;

namespace Server.Application.Features.Role.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
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
