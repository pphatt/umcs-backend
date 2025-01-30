using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionLikes")]
public class ContributionLike : BaseEntity
{
    [Required]
    public required Guid ContributionId { get; set; }

    [Required]
    public required Guid UserId { get; set; }
}
