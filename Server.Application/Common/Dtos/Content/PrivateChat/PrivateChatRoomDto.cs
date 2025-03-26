namespace Server.Application.Common.Dtos.Content.PrivateChat;

public class PrivateChatRoomDto
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Avatar { get; set; } = default!;

    public DateTime LastActivity { get; set; }

    public string LastMessageFromWho { get; set; } = default!;

    public string LastMessage { get; set; }

    public DateTime LastTimeTexting { get; set; }
}
