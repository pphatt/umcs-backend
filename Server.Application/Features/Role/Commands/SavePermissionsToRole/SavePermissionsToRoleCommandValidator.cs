using FluentValidation;

namespace Server.Application.Features.Role.Commands.SavePermissionsToRole;

public class SavePermissionsToRoleCommandValidator : AbstractValidator<SavePermissionsToRoleCommand>
{
    public SavePermissionsToRoleCommandValidator()
    {
    }
}
