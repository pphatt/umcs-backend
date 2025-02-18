using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.AcademicYears.BulkDeleteAcademicYears;

public class BulkDeleteAcademicYearsRequest
{
    [Required]
    public List<Guid> AcademicIds { get; set; } = default!;
}
