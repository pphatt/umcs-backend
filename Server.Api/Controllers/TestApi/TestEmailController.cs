using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Contracts.Common.Email;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestEmailController : TestApiController
{
    private readonly IEmailService _emailService;

    public TestEmailController(IEmailService emailSerivce)
    {
        _emailService = emailSerivce;
    }

    [HttpGet("email")]
    public async Task<IActionResult> TestEmail()
    {
        var emailRequest = new MailRequest
        {
            ToEmail = "phatvu080903@gmail.com",
            Subject = "Test",
            Body = "Test",
        };

        await _emailService.SendEmailAsync(emailRequest);

        return Ok("Email send successfully.");
    }
}
