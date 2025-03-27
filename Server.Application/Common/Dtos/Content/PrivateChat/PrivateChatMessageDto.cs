namespace Server.Application.Common.Dtos.Content.PrivateChat;

public class PrivateChatMessageDto
{
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
    public string SenderAvatar { get; set; }
    public string ReceiverAvatar { get; set; }
    public DateTime DateCreated { get; set; }
}
