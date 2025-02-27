using Microsoft.AspNetCore.Http.Features;

namespace Server.Contracts.Contributions.ApproveContribution;

public class ApproveContributionRequest
{
    public required Guid Id { get; set; }
}
