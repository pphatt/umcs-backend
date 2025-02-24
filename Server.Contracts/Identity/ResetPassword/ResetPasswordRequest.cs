using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Identity.ResetPassword;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; } = default!;

    [Required]
    public string Password { get; set; } = default!;
}
