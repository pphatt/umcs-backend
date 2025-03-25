using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("PrivateChatRooms")]
public class PrivateChatRoom : BaseEntity
{
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public Guid User2LastActivity { get; set; }
}
