namespace Server.Application.Common.Dtos.Content.Contribution;

public class ContributionActivityLogDto
{
    public Guid Id { get; set; }

    public Guid ContributionId { get; set; }

    public string ContributionTitle { get; set; } = default!;

    public Guid UserId { get; set; }

    public string Username { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string FromStatus { get; set; } = default!;

    public string ToStatus { get; set; } = default!;

    public DateTime DateCreated { get; set; }
}
