namespace Server.Contracts.Identity.CreateUser;

public record CreateUserRequest(
    string Email,
    string UserName,
    //string Password,
    string? FirstName,
    string? LastName,
    Guid FacultyId,
    Guid RoleId,
    bool IsActive
);
