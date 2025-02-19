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

        public static Error CannotFound => Error.NotFound(
            code: "Contribution.CannotFound",
            description: "Contribution not found."
        );

        public static Error SlugExists => Error.Validation(
            code: "Contribution.SlugExists",
            description: "Please enter another title. Current title is already exist."
        );

        public static Error NotConfirmed => Error.Validation(
            code: "Contribution.NotConfirmed",
            description: "Please accept term and condition before submitting."
        );

        public static Error AlreadyDeleted => Error.Validation(
            code: "Contribution.AlreadyDeleted",
            description: "Contribution has already been deleted."
        );

        public static Error AlreadyApproved => Error.Validation(
            code: "Contribution.AlreadyApproved",
            description: "Contribution has already been approved."
        );

        public static Error AlreadyRejected => Error.Validation(
            code: "Contribution.AlreadyRejected",
            description: "Contribution has already been rejected."
        );

        public static Error CannotSubmit => Error.Validation(
            code: "Contribution.CannotSubmit",
            description: "Submission due date have passed."
        );
    };
}
