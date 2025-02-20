using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Contributions.CreateContribution;

public class CreateContributionRequest
{
    public required string Title { get; set; }

    public IFormFile? Thumbnail { get; set; }

    public string Content { get; set; }

    public string ShortDescription { get; set; }

    public List<IFormFile>? Files { get; set; }

    public required bool IsConfirmed { get; set; }
}
