namespace Server.Contracts.Authentication;

public record AuthenticationResponse(string AccessToken, string RefreshToken);
