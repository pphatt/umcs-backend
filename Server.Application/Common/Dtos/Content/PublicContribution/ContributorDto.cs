namespace Server.Application.Common.Dtos.Content.PublicContribution;

public class ContributorDto
{
    public string Username { get; set; }
    public string FacultyName { get; set; }
    public string Avatar { get; set; }
    public int TotalLikes { get; set; }
    public int TotalContributions { get; set; }
}
