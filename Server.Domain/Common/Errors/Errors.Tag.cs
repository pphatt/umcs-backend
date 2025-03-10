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
    }
}
