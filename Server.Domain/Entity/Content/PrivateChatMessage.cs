using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("PrivateChatMessages")]
public class PrivateChatMessage : BaseEntity
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid ChatRoomId { get; set; }
    public string Content { get; set; } = default!;
    public bool HasRed { get; set; }
}
