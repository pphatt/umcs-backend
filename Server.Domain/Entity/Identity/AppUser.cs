using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Identity;

[Table("AppUsers")]
public class AppUser : IdentityUser<Guid>
{
    [MaxLength(100)]
    public string? FirstName { get; set; } = default!;

    [MaxLength(100)]
    public string? LastName { get; set; } = default!;

    [Required]
    public Guid? FacultyId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? Dob { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }
}
