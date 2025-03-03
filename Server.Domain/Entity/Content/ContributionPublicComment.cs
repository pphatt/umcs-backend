using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionPublicComments")]
public class ContributionPublicComment : BaseEntity
{
    [Required]
    public Guid ContributionId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Content { get; set; } = default!;
}
