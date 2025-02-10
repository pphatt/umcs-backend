using System.ComponentModel;

namespace Server.Contracts.Identity.UpdateUser;

public record UpdateUserRequest(
    [property: DefaultValue("")] Guid Id,
    [property: DefaultValue("")] string? FirstName,
    [property: DefaultValue("")] string? LastName,
    [property: DefaultValue("")] Guid FacultyId,
    [property: DefaultValue("")] Guid RoleId,
    [property: DefaultValue("")] DateTime? Dob,
    [property: DefaultValue("")] bool IsActive
);
