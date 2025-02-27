using Server.Application.Common.Dtos.Media;

namespace Server.Application.Common.Dtos.Content.Contribution;

public class ContributionInListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public List<FileDto> Thumbnails { get; set; }
    public List<FileDto> Files { get; set; }
    public string Status { get; set; }
    public string Username { get; set; }
    public string FacultyName { get; set; }
    public string AcademicYear { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime? PublicDate { get; set; }
    public string? ShortDescription { get; set; }
    public string? RejectReason { get; set; }
    public bool? GuestAllowed { get; set; }
    public string? Avatar { get; set; }
}
