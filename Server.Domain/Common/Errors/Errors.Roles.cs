using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class Roles
    {
        public static Error NameDuplicated => Error.Validation(
            code: "Roles.NameDuplicated",
            description: "Role name is duplicated."
        );

        public static Error DisplayNameDuplicated => Error.Validation(
            code: "Roles.DisplayNameDuplicated",
            description: "Role display name is duplicated"
        );

        public static Error EmptyId => Error.Validation(
            code: "Roles.EmptyId",
            description: "Role id cannot be empty."
        );

        public static Error CannotFound => Error.NotFound(
            code: "Roles.NotFound",
            description: "Role cannot found."
        );
    }
}
