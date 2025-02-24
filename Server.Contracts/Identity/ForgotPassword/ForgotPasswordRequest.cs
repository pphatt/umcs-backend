using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Identity.ForgotPassword;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}
