using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class Contribution
    {
        public static Error AcademicYearNotFound => Error.Validation(
            code: "Contribution.AcademicYearNotFound",
            description: "Academic year is not found to create contribution."
        );

        public static Error NotConfirmed => Error.Validation(
            code: "Contribution.NotConfirmed",
            description: "Please accept term and condition before submitting."
        );

        public static Error CannotSubmit => Error.Validation(
            code: "Contribution.CannotSubmit",
            description: "Submission due date have passed."
        );
    }
}
