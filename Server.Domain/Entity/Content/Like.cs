using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Likes")]
public class Like : BaseEntity
{
    [Required]
    public required Guid ContributionId { get; set; }

    [Required]
    public required Guid UserId { get; set; }
}
