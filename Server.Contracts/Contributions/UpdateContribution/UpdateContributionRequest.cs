using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Contributions.UpdateContribution;

public class UpdateContributionRequest
{
    public string Title { get; set; }

    public IFormFile? Thumbnail { get; set; }

    public string Content { get; set; }

    public string ShortDescription { get; set; }

    public List<IFormFile>? Files { get; set; }

    public bool IsConfirmed { get; set; }
}
