using Server.Application.Common.Dtos.Content.PublicContribution;

namespace Server.Application.Common.Dtos.Content.PrivateChat;

public class PrivateChatRoomDto
{
    public Guid ChatId { get; set; }
    public Guid CurrentUserId { get; set; }
    public Guid ReceiverId { get; set; }
    public string? Username { get; set; }
    public string? Avatar { get; set; } = default!;
    public string? Role { get; set; }
    public bool IsOnline { get; set; }
    public DateTime LastActivity { get; set; }
    public string LastMessageFromWho { get; set; } = default!;
    public string LastMessage { get; set; }
    public DateTime LastTimeTexting { get; set; }
}
