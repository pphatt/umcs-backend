using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Identity.EditUserProfile;

public class EditUserProfileRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? Dob { get; set; }

    public string? PhoneNumber { get; set; }

    public IFormFile? Avatar { get; set; }
}
