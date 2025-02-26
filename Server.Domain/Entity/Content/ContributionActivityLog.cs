using Microsoft.EntityFrameworkCore;
using Server.Domain.Common.Enums;
using Server.Domain.Entity.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionActivityLogs")]
[Index(nameof(ContributionId))]
public class ContributionActivityLog : BaseEntity
{
    [Required]
    public Guid ContributionId { get; set; }

    public Contribution Contribution { get; set; } = default!;

    [MaxLength(256)]
    public string ContributionTitle { get; set; } = default!;

    [Required]
    public Guid CoordinatorId { get; set; }

    public AppUser Coordinator { get; set; } = default!;

    [Required]
    public string CoordinatorUsername { get; set; } = default!;

    [MaxLength(500)]
    public string Description { get; set; } = default!;

    [Required]
    public ContributionStatus FromStatus { get; set; }

    [Required]
    public ContributionStatus ToStatus { get; set; }
}
