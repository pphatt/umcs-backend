namespace Server.Contracts.PublicContributions.RevokeAllowGuestWithManyContributions;

public class RevokeAllowGuestWithManyContributionsRequest
{
    public List<Guid> ContributionIds { get; set; } = default!;
}
