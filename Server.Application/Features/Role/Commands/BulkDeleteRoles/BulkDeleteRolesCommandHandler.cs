using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Commands.BulkDeleteRoles;

public class BulkDeleteRolesCommandHandler : IRequestHandler<BulkDeleteRolesCommand, ErrorOr<ResponseWrapper>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public BulkDeleteRolesCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(BulkDeleteRolesCommand request, CancellationToken cancellationToken)
    {
        var rolesIds = request.RoleIds;
        var successfullyDeletedItems = new List<Guid>();

        foreach (var roleId in rolesIds)
        {
            if (string.IsNullOrWhiteSpace(roleId.ToString()))
            {
                return Errors.Roles.EmptyId;
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());

            if (role is null)
            {
                return Errors.Roles.CannotFound;
            }

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                return result.Errors.Select(error => Error.Validation(code: error.Code, description: error.Description)).ToArray();
            }

            successfullyDeletedItems.Add(roleId);
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = $"Successfully deleted {successfullyDeletedItems.Count} roles."
        };
    }
}
