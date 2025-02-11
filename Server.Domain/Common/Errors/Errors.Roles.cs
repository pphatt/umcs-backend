using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class Roles
    {
        public static Error CannotFound => Error.NotFound(
            code: "Roles.NotFound",
            description: "Role cannot found."
        );
    }
}
