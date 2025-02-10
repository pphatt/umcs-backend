using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class Faculty
    {
        public static Error NotFound => Error.NotFound(
            code: "Faculty.NotFound",
            description: "Faculty cannot found."
        );
    }
}
