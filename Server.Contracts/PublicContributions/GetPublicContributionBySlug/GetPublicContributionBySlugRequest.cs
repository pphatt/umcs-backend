using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.PublicContributions.GetPublicContributionBySlug;

public class GetPublicContributionBySlugRequest
{
    [FromRoute]
    public string Slug { get; set; } = default!;
}
