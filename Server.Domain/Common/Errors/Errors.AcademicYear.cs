using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class AcademicYears
    {
        public static Error InvalidName => Error.Validation(
            code: "AcademicYear.InvalidName",
            description: "Academic year's name is invalid."
        );

        public static Error CannotFound => Error.NotFound(
            code: "AcademicYear.NotFound",
            description: "Academic year is not found."
        );

        public static Error DuplicateName => Error.Validation(
            code: "AcademicYear.DuplicateName",
            description: "Academic year's name already exists."
        );

        public static Error HasContributions => Error.Validation(
            code: "AcademicYear.HasContributions",
            description: "Cannot delete due to contributions are still in the academic year."
        );
    }
}
