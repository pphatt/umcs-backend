using System.ComponentModel.DataAnnotations;

namespace Server.Contracts.Identity.ValidateForgotPasswordToken;

public class ValidateForgotPasswordTokenRequest
{
    [Required]
    public string Token { get; set; }
}
