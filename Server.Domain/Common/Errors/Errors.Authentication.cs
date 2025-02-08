using ErrorOr;

namespace Server.Domain.Common.Errors;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials => Error.Unauthorized(
            code: "Authentication.InvalidCredentials",
            description: "Invalid Credentials."
        );
    }
}
