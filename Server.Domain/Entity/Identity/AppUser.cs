using Microsoft.AspNetCore.Identity;
using Server.Domain.Entity.Content;
using Server.Domain.Entity.Token;
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

    public bool IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime? Dob { get; set; }

    [MaxLength(500)]
    public string? Avatar { get; set; } = default!;

    [MaxLength(500)]
    public string? AvatarPublicId { get; set; } = default!;

    public RefreshToken RefreshToken { get; set; } = default!;

    public ICollection<ContributionPublicRating> Ratings { get; set; }
    public ICollection<ContributionPublicReadLater> ReadLaters { get; set; }
    public ICollection<ContributionPublicBookmark> Bookmarks { get; set; }
}
