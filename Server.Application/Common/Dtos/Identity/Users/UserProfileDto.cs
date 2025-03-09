namespace Server.Application.Common.Dtos.Identity.Users;

public class UserProfileDto
{
    public Guid Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Faculty { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? Dob { get; set; }

    public string? Avatar { get; set; }

    public DateTime DateCreated { get; set; }
}
