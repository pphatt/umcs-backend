using Server.Domain.Entity.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionPublicReadLaters")]
public class ContributionPublicReadLater : BaseEntity
{
    [Required]
    public Guid ContributionId { get; set; }

    public ContributionPublic ContributionPublic { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public AppUser User { get; set; }
}
