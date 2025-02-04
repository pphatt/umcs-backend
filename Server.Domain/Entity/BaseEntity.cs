using System.ComponentModel.DataAnnotations;

namespace Server.Domain.Entity;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateDeleted { get; set; }
}
