using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Identity.UpdateUser;

public class UpdateUserRequest
{
    [FromRoute]
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public Guid FacultyId { get; set; }

    public Guid RoleId { get; set; }

    DateTime? Dob { get; set; }

    bool IsActive { get; set; }
};
