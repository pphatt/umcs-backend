using Microsoft.EntityFrameworkCore;
using Server.Domain.Entity.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Token;

[Table("Token")]
[Index(nameof(Token), IsUnique = true)]
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = default!;
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; } = default!;
}
