namespace Server.Application.Common.Dtos.Content.Contribution;

public class UngradedContributionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Username { get; set; }
    public string FacultyName { get; set; }
    public string AcademicYear { get; set; }
    public DateTime SubmissionDate { get; set; }
    public string? ShortDescription { get; set; }
    public string? Avatar { get; set; }
}
