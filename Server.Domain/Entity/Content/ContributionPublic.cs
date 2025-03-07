using Microsoft.EntityFrameworkCore;
using Server.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionPublics")]
[Index(nameof(Slug), IsUnique = true)]
public class ContributionPublic : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Username { get; set; } = default!;

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

    [MaxLength(500)]
    public string Avatar { get; set; } = default!;

    [Required]
    public string FacultyName { get; set; } = default!;

    [Required]
    public ContributionStatus Status { get; set; }

    [Required]
    public DateTime SubmissionDate { get; set; }

    public DateTime? PublicDate { get; set; }

    public bool IsCoordinatorCommented { get; set; } = false;

    public bool AllowedGuest { get; set; } = false;

    public Guid? CoordinatorApprovedId { get; set; }

    public List<ContributionTag> ContributionTags { get; set; } = new();

    public int LikeQuantity { get; set; } = 0;

    public int Views { get; set; } = 0;

    public ICollection<ContributionPublicReadLater> ReadLaters { get; set; }
}
