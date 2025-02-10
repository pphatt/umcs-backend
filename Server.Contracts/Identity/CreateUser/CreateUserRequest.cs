using System.ComponentModel;

namespace Server.Contracts.Identity.CreateUser;

public record CreateUserRequest(
    [property: DefaultValue("")] string Email,
    [property: DefaultValue("")] string UserName,
    //string Password,
    [property: DefaultValue("")] string? FirstName,
    [property: DefaultValue("")] string? LastName,
    [property: DefaultValue("")] Guid FacultyId,
    [property: DefaultValue("")] Guid RoleId,
    [property: DefaultValue("")] bool IsActive
);
