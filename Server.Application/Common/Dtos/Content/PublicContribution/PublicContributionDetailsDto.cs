using Server.Application.Common.Dtos.Media;

namespace Server.Application.Common.Dtos.Content.PublicContribution;

public class PublicContributionDetailsDto : PublicContributionInListDto
{
    public List<FileDto>? Files { get; set; }

    public string Content { get; set; }

    public bool AllowedGuest { get; set; }
}
