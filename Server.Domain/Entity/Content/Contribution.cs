using Microsoft.EntityFrameworkCore;
using Server.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Contributions")]
[Index(nameof(Slug), IsUnique = true)]
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

    public ICollection<File> Files { get; set; } = new List<File>();

    [Required]
    [MaxLength(256)]
    public string Title { get; set; } = default!;

    [Required]
    [Column(TypeName = "varchar(256)")]
    public string Slug { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;

    [Required]
    public string ShortDescription { get; set; }

    [Required]
    public bool IsConfirmed { get; set; }

    [Required]
    public ContributionStatus Status { get; set; }

    [Required]
    public DateTime SubmissionDate { get; set; }

    [Required]
    public DateTime PublicDate { get; set; }

    public bool AllowedGuest { get; set; } = false;

    public List<ContributionTag> ContributionTags { get; set; } = new();
}
