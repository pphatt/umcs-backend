using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Contributions.GetContributionBySlug;

public class GetContributionBySlugRequest
{
    [Required]
    public string Slug { get; set; } = default!;
}
