using System.ComponentModel;

namespace Server.Domain.Common.Constants.Authorization;

public static class Permissions
{
    public static class Roles
    {
        [Description("Create New Role")]
        public const string Create = "Permissions.Roles.Create";

        [Description("View Roles")]
        public const string View = "Permissions.Roles.View";

        [Description("Update Role")]
        public const string Edit = "Permissions.Roles.Edit";

        [Description("Delete Role")]
        public const string Delete = "Permissions.Roles.Delete";
    }

    public static class Users
    {
        [Description("Create New User")]
        public const string Create = "Permissions.Users.Create";

        [Description("View Users")]
        public const string View = "Permissions.Users.View";

        [Description("Update User")]
        public const string Edit = "Permissions.Users.Edit";

        [Description("Delete User")]
        public const string Delete = "Permissions.Users.Delete";
    }
}
