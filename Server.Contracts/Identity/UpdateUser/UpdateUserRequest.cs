namespace Server.Contracts.Identity.UpdateUser;

public record UpdateUserRequest(
    Guid Id,
    string? FirstName,
    string? LastName,
    Guid FacultyId,
    Guid RoleId,
    DateTime? Dob,
    bool IsActive
);
