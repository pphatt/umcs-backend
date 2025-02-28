namespace Server.Contracts.PublicContributions.AllowGuestWithManyContributions;

public class AllowGuestWithManyContributionsRequest
{
    public List<Guid> ContributionIds { get; set; } = default!;
}
