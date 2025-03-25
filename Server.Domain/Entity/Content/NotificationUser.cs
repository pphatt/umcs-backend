using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Entity.Identity;

namespace Server.Domain.Entity.Content;

[Table("NotificationUsers")]
public class NotificationUser : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid NotificationId { get; set; }

    [Required]
    public bool HasRed { get; set; } = false;

    public AppUser User { get; set; } = default!;
    public Notification Notification { get; set; } = default!;
}
