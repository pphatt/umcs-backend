using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Interfaces.Services.Email;
using Server.Contracts.Common.Email;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.TestApi;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IEmailService _emailService;

    public TestController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet("auth")]
    [Authorize]
    [Authorize(Permissions.Users.View)]
    public IActionResult TestAuth()
    {
        return Ok("Access auth route successfully.");
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
