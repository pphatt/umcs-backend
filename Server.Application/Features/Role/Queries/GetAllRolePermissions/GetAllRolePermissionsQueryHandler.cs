using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Common.Extensions;
using Server.Application.Wrapper;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Errors;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Queries.GetAllRolePermissions;

public class GetAllRolePermissionsQueryHandler : IRequestHandler<GetAllRolePermissionsQuery, ErrorOr<ResponseWrapper<PermissionsDto>>>
{
    private readonly RoleManager<AppRole> _roleManager;

    public GetAllRolePermissionsQueryHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<ErrorOr<ResponseWrapper<PermissionsDto>>> Handle(GetAllRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
        {
            return Errors.Roles.CannotFound;
        }

        var allPermissions = typeof(Permissions).GetNestedTypes().ToList();
        var allPermissionsList = new List<RoleClaimsDto>();

        foreach (var permission in allPermissions)
        {
            allPermissionsList.GetPermissionByType(permission);
        }

        var currentRolePermissions = await _roleManager.GetClaimsAsync(role);
        var currentRolePermissionsList = currentRolePermissions.Select(x => x.Value).ToHashSet();

        foreach (var permission in allPermissionsList)
        {
            if (currentRolePermissionsList.Contains(permission.Value!.ToString()))
            {
                permission.Selected = true;
            }
        }

        return new ResponseWrapper<PermissionsDto>
        {
            IsSuccessful = true,
            ResponseData = new PermissionsDto
            {
                RoleId = role.Id.ToString(),
                RoleClaims = allPermissionsList
            }
        };
    }
}
