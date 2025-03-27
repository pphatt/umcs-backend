using ErrorOr;

using Server.Domain.Entity.Content;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public partial class Notification
    {
        public static Error CannotFound => Error.NotFound(
            code: "Notification.CannotFound",
            description: "Notification cannot found"
        );
    }
}
