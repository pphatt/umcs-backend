using Server.Application.Common.Dtos.Content.Comment;

namespace Server.Application.Common.Dtos.Content.PublicContribution;

public class PublicContributionWithCommentsDto : PublicContributionDto
{
    public List<CommentDto> Comments { get; set; }
}
