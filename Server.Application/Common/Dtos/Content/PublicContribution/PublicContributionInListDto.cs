using Server.Application.Common.Dtos.Media;
using Server.Application.Common.Extensions;
using Server.Domain.Common.Enums;

namespace Server.Application.Common.Dtos.Content.PublicContribution;

public class PublicContributionInListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public List<FileDto> Thumbnails { get; set; }
    public string Username { get; set; }
    public string FacultyName { get; set; }
    public string AcademicYearName { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime? PublicDate { get; set; }
    public DateTime? DateEdited { get; set; }
    public int Like { get; set; } = 0;
    public int View { get; set; } = 0;
    public string? ShortDescription { get; set; }
    public string? Avatar { get; set; }
    public string? WhoApproved { get; set; }
    public bool DidLike { get; set; } = false;

    public string? Status { get; set; } = ContributionStatus.Approve.ToStringValue();
}
