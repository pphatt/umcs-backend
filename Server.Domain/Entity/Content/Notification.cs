using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Server.Domain.Entity.Identity;

namespace Server.Domain.Entity.Content;

[Table("Notifications")]
public class Notification : BaseEntity
{
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = default!;

    [Required]
    [StringLength(512)]
    public string Content { get; set; } = default!;

    [StringLength(256)]
    public string Slug { get; set; } = default!;

    [StringLength(256)]
    public string Type { get; set; } = default!;

    [Required]
    public Guid UserId { get; set; } // SenderId

    [StringLength(256)]
    public string Username { get; set; } = default!;

    [StringLength(256)]
    public string Avatar { get; set; } = default!;

    public AppUser Sender { get; set; } = default!;
    public ICollection<NotificationUser> NotificationUsers { get; set; } = default!;
}
