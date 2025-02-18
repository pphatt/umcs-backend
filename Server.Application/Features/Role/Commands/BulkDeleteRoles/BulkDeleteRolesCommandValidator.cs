using FluentValidation;

namespace Server.Application.Features.Role.Commands.BulkDeleteRoles;

public class BulkDeleteRolesCommandValidator : AbstractValidator<BulkDeleteRolesCommand>
{
    public BulkDeleteRolesCommandValidator()
    {
    }
}
