using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("ContributionRejections")]
[Index(nameof(ContributionId), IsUnique = true)]
public class ContributionRejection : BaseEntity
{
    public Guid ContributionId { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = default!;
}
