using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("AcademicYears")]
public class AcademicYear : BaseEntity
{
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; } = default!;

    [Required]
    public required DateTime StartClosureDate { get; set; }

    [Required]
    public required DateTime EndClosureDate { get; set; }

    [Required]
    public required DateTime FinalClosureDate { get; set; }

    [Required]
    public required Guid UserIdCreated { get; set; }

    [Required]
    public required bool IsActive { get; set; } = false;

    public List<Contribution> Contributions { get; set; } = new();
}
