namespace Server.Contracts.PrivateChats.SendChatMessage;

public class SendChatMessageRequest
{
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
}
