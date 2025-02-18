using FluentValidation;

namespace Server.Application.Features.Identity.Commands.BulkDeleteUsers;

public class BulkDeleteUsersCommandValidator : AbstractValidator<BulkDeleteUsersCommand>
{
    public BulkDeleteUsersCommandValidator()
    {
    }
}
