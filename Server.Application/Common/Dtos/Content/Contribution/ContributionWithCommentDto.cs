using Server.Application.Common.Dtos.Content.Comment;

namespace Server.Application.Common.Dtos.Content.Contribution;

public class ContributionWithCommentDto : ContributionDto
{
    public List<CommentDto> Comments { get; set; }
}
