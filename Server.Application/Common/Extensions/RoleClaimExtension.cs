using Server.Application.Common.Dtos.Authorization;
using Server.Domain.Common.Constants.Authorization;
using System.Reflection;

namespace Server.Application.Common.Extensions;

public static class RoleClaimExtension
{
    public static void GetPermissionByType(this List<RoleClaimsDto> allPermissions, Type policy)
    {
        // get all the static const out of the type ("Roles", "User")
        FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            string value = field.GetValue(null)!.ToString()!;

            //var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);

            //if (attributes.Any())
            //{
            //    var description = (DescriptionAttribute)attributes[0];
            //    displayName = description.Description;
            //}

            allPermissions.Add(new RoleClaimsDto
            {
                Value = field.GetValue(null)!.ToString(),
                Type = UserClaims.Permissions,
                //DisplayName = displayName
            });
        }
    }
}
