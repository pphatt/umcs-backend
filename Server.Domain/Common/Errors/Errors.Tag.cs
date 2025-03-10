using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class Tags
    {
        public static Error AlreadyExist => Error.Validation(
            code: "Tag.AlreadyExist",
            description: "Tag already exist."
        );

        public static Error CannotFound => Error.NotFound(
            code: "Tag.CannotFound",
            description: "Tag cannot found."
        );

        public static Error AlreadyDeleted => Error.Failure(
            code: "Tag.AlreadyDeleted",
            description: "Tag is already deleted."
        );
    }
}
