using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Domain.Entity.Identity;

[Table("AppRoles")]
public class AppRoles : IdentityRole<Guid>
{
    public string DisplayName { get; set; } = default!;
}
