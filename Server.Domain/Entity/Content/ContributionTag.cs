using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionTags")]
public class ContributionTag : BaseEntity
{
    [Required]
    public Guid ContributionId { get; set; }
    public Contribution Contribution { get; set; } = default!;

    [Required]
    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}
