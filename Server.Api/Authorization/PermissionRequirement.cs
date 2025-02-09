using Microsoft.AspNetCore.Authorization;

namespace Server.Api.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string Permission)
    {
        this.Permission = Permission;
    }
}
