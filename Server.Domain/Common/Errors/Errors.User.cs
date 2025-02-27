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

        public static Error CoordinatorCannotFound => Error.NotFound(
            code: "Coordinator.CannotFound",
            description: "Coordinator cannot found."
        );

        public static Error InactiveOrLockedOut => Error.Validation(
            code: "User.InactiveOrLockedOut",
            description: "User is inactive or locked out. Contact admin for more information."
        );

        public static Error FailResetPassword => Error.Failure(
            code: "User.FailResetPassword",
            description: "Error occurs while reseting password."
        );

        public static Error NotOwnedContribution => Error.Validation(
            code: "User.NotOwnedContribution",
            description: "This is not your contribution"
        );
    }
}
