using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Identity.CreateUser;

public class CreateUserRequest
{
    public string Email { get; set; } = default!;

    public string UserName { get; set; } = default!;

    //public string Password { get; set; } = default!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid FacultyId { get; set; }

    public Guid RoleId { get; set; }

    public IFormFile? Avatar { get; set; }

    public bool IsActive { get; set; }
};
