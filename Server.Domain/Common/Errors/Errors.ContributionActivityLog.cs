using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class ContributionActivity
    {
        public static Error CannotFound => Error.NotFound(
            code: "ContributionActivity.CannotFound",
            description: "Contribution Activity not found."
        );
    }
}
