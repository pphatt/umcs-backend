using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class User
    {
        public static Error DuplicateEmail => Error.Validation(
            code: "User.DuplicateEmail",
            description: "User's email already exists."
        );

        public static Error CannotFound => Error.NotFound(
            code: "User.CannotFound",
            description: "User cannot found."
        );

        public static Error InactiveOrLockedOut => Error.Validation(
            code: "User.InactiveOrLockedOut",
            description: "User is inactive or locked out. Contact admin for more information."
        );
    }
}
