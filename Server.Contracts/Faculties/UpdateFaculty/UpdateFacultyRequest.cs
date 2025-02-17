using Microsoft.AspNetCore.Mvc;

namespace Server.Contracts.Faculties.UpdateFaculty;

public class UpdateFacultyRequest
{
    [FromRoute]
    public Guid Id { get; set; }

    public required string Name { get; set; } = default!;
}
