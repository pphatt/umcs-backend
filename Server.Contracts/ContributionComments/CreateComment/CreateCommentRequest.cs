using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.ContributionComments.CreateComment;

public class CreateCommentRequest
{
    [FromRoute]
    public Guid ContributionId { get; set; }

    [FromForm]
    public string Content { get; set; } = default!;
}
