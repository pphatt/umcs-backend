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

        public static Error NotBelongToFaculty => Error.Validation(
            code: "Contribution.NotBelongToFaculty",
            description: "This contribution is not belong to your faculty."
        );

        public static Error NotPublicYet => Error.Validation(
            code: "Contribution.NotPublicYet",
            description: "This contribution is not found or not public yet."
        );

        public static Error NotAllowYet => Error.Validation(
            code: "Contribution.NotAllowYet",
            description: "This contribution has not allowed guest to view yet."
        );

        public static Error AlreadyAllowGuest => Error.Validation(
            code: "Contribution.AlreadyAllowGuest",
            description: "This contribution has already allowed guest to view it."
        );

        public static Error NotAllowed => Error.Validation(
            code: "Contribution.NotAllowed",
            description: "You do not have permission to access this contribution."
        );

        public static Error NotBelongTo => Error.Validation(
            code: "Contribution.NotBelongTo",
            description: "This contribution is not belong to you."
        );

        public static Error NoFilesFound => Error.NotFound(
            code: "Contribution.NoFilesFound",
            description: "Contribution do not have any files."
        );

        public static Error CannotCommentOnContributionAlreadyGraded => Error.Validation(
            code: "Contribution.CannotCommentOnContributionAlreadyGraded",
            description: "Cannot comment on contribution that already graded."
        );

        public static Error CannotCommentOnNotPublicContribution => Error.Validation(
            code: "Contribution.CannotCommentOnNotPublicContribution",
            description: "Cannot comment on contribution that not public."
        );
    };
}
