namespace Server.Application.Common.Dtos.Content.PrivateChat;

public class NewUserConnectedDto
{
    public string UserId { get; set; }

    public string Username { get; set; } = default!;
}
