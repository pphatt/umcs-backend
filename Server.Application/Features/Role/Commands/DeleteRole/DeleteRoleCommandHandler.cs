using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Commands.DeleteRole;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, ErrorOr<ResponseWrapper>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public DeleteRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id.ToString()))
        {
            return Errors.Roles.EmptyId;
        }

        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Delete role successfully."
        };
    }
}
