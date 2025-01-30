using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Tags")]
public class Tag : BaseEntity
{
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; } = default!;

    public List<ContributionTag> ContributionTags { get; set; } = new();
}
