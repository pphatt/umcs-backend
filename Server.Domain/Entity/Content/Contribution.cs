using Server.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Contributions")]
public class Contribution : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid FacultyId { get; set; }

    [ForeignKey("FacultyId")]
    public Faculty Faculty { get; set; } = default!;

    [Required]
    public Guid AcademicYearId { get; set; }

    [ForeignKey("AcademicYearId")]
    public AcademicYear AcademicYear { get; set; } = default!;

    [Required]
    [MaxLength(256)]
    public required string Title { get; set; } = default!;

    [Required]
    public required string Content { get; set; } = default!;

    [Required]
    public required bool IsConfirmed { get; set; }

    [Required]
    public required ContributionStatus Status { get; set; }

    [Required]
    public DateTime SubmissionDate { get; set; }

    [Required]
    public DateTime PublicDate { get; set; }

    public bool AllowedGuest { get; set; } = false;

    public List<ContributionTag> ContributionTags { get; set; } = new();
}
