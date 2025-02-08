using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class RefreshToken
    {
        public static Error Invalid => Error.Validation(
            code: "RefreshToken.Invalid",
            description: "Invalid Refresh Token."
        );

        public static Error Expired => Error.Validation(
            code: "RefreshToken.Expired",
            description: "Refresh Token has been expired."
        );
    }
}
