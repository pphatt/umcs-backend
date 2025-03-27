namespace Server.Application.Common.Dtos.Content.PrivateChat;

public class NewUserDisconnectedDto
{
    public string UserId { get; set; }

    public string Username { get; set; } = default!;
}
