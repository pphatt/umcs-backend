namespace Server.Contracts.Contributions.RejectContribution;

public class RejectContributionRequest
{
    public Guid Id { get; set; }

    public string Reason { get; set; } = "Not qualify enough.";
}
