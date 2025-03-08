using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.PublicContributions.RatePublicContribution;

public class RatePublicContributionRequest
{
    [FromRoute]
    public Guid ContributionId { get; set; }

    [FromForm]
    public double Rating { get; set; }
}
