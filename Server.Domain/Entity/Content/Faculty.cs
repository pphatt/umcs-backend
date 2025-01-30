using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Content;

[Table("Faculties")]
public class Faculty : BaseEntity
{
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; } = default!;

    public List<Contribution> Contributions = new();
}
