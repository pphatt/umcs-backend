using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class Faculty
    {
        public static Error InvalidName => Error.Validation(
            code: "Faculty.InvalidName",
            description: "Faculty's name is invalid."
        );

        public static Error CannotFound => Error.NotFound(
            code: "Faculty.NotFound",
            description: "Faculty cannot found."
        );

        public static Error HasUserIn => Error.Validation(
            code: "Faculty.HasUserIn",
            description: "Users are found in the faculty so we cannot delete it."
        );

        public static Error DuplicateName => Error.Validation(
            code: "Faculty.DuplicateName",
            description: "Faculty's name already exists."
        );
    }
}
