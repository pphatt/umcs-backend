namespace Server.Contracts.Authentication.Login;

public record LoginResult(string AccessToken, string RefreshToken);
