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

    public static class Faculties
    {
        [Description("Create New Faculty")]
        public const string Create = "Permissions.Faculties.Create";

        [Description("View Faculties")]
        public const string View = "Permissions.Faculties.View";

        [Description("Update Faculty")]
        public const string Edit = "Permissions.Faculties.Edit";

        [Description("Delete Faculty")]
        public const string Delete = "Permissions.Faculties.Delete";
    }

    public static class AcademicYears
    {
        [Description("Create New Academic Year")]
        public const string Create = "Permissions.AcademicYears.Create";

        [Description("View Academic Years")]
        public const string View = "Permissions.AcademicYears.View";

        [Description("Update Academic Year")]
        public const string Edit = "Permissions.AcademicYears.Edit";

        [Description("Delete Academic Year")]
        public const string Delete = "Permissions.AcademicYears.Delete";
    }

    public static class Contributions
    {
        [Description("Create New Contribution")]
        public const string Create = "Permissions.Contributions.Create";

        [Description("View Contribution")]
        public const string View = "Permissions.Contributions.View";

        [Description("Update Contribution")]
        public const string Edit = "Permissions.Contributions.Edit";

        [Description("Delete Contribution")]
        public const string Delete = "Permissions.Contributions.Delete";

        [Description("Approve Contribution")]
        public const string Approve = "Permissions.Contributions.Approve";

        [Description("Reject Contribution")]
        public const string Reject = "Permissions.Contributions.Reject";
    }

    public static class ManageContributions
    {
        [Description("View Manage Contributions")]
        public const string Manage = "Permissions.ManageContributions.View";
    }

    public static class SettingGAC
    {
        [Description("Manage Guest Setting Access Control")]
        public const string Manage = "Permissions.SettingGAC.Manage";
    }

    public static class ActivityLogs
    {
        [Description("View Activity Logs")]
        public const string View = "Permissions.ActivityLogs.View";
    }
}
