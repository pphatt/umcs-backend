using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;
using System.Security.Claims;

namespace Server.Application.Features.Role.Commands.SavePermissionsToRole;

public class SavePermissionsToRoleCommandHandler : IRequestHandler<SavePermissionsToRoleCommand, ErrorOr<ResponseWrapper>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public SavePermissionsToRoleCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper>> Handle(SavePermissionsToRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.RoleId);

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        var rolePermissions = await _roleManager.GetClaimsAsync(role);

        foreach (var permission in rolePermissions)
        {
            await _roleManager.RemoveClaimAsync(role, permission);
        }

        var requestedPermissions =
            request.RoleClaims
                   .Where(x => x.Selected)
                   .Select(x => x.Value)
                   .ToHashSet();

        foreach (var permission in requestedPermissions)
        {
            await _roleManager.AddClaimAsync(role, new Claim(UserClaims.Permissions, permission!));
        }

        return new ResponseWrapper
        {
            IsSuccessful = true,
            Message = "Save permissions successfully."
        };
    }
}
