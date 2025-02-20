using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Files")]
public class File : BaseEntity
{
    [Required]
    public Guid ContributionId { get; set; }

    [ForeignKey("ContributionId")]
    public Contribution Contribution { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Type { get; set; }

    [Required]
    [MaxLength(256)]
    public string Name { get; set; }

    [Required]
    [MaxLength(500)]
    public string Path { get; set; }

    [Required]
    [MaxLength(500)]
    public string PublicId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Extension { get; set; }
}
