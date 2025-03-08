using Server.Domain.Entity.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionPublicRatings")]
public class ContributionPublicRating : BaseEntity
{
    public Guid ContributionId { get; set; }
    public ContributionPublic ContributionPublic { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public double Rating { get; set; } = 0;
}
