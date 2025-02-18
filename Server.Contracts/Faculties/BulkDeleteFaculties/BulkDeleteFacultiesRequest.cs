namespace Server.Contracts.Faculties.BulkDeleteFaculties;

public class BulkDeleteFacultiesRequest
{
    public List<Guid> FacultyIds { get; set; } = default!;
}
