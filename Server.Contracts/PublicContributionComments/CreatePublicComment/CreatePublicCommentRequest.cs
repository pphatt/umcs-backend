using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.PublicContributionComments.CreatePublicComment;

public class CreatePublicCommentRequest
{
    [FromRoute]
    public Guid ContributionId { get; set; }

    [FromForm]
    public string Content { get; set; }
}
