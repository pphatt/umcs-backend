using Server.Application.Common.Dtos.Media;

namespace Server.Application.Common.Dtos.Content.Contribution;

public class ContributionDto : ContributionInListDto
{
    public string Content { get; set; } = default!;

    public DateTime? DateUpdated { get; set; }
}
