using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionComments")]
public class ContributionComment : BaseEntity
{
    [Required]
    public required Guid ContributionId { get; set; }

    [Required]
    public required Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Content { get; set; } = default!;
}
