using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Tags.UpdateTag;

public class UpdateTagRequest
{
    public Guid Id { get; set; }

    [FromForm]
    public string TagName { get; set; } = default!;
}
