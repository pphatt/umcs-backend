using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers;

[ApiController]
[Route("/authentication")]
public class AuthorizationController : ControllerBase
{
    [HttpPost("/login")]
    public IActionResult Login()
    {
        return Ok();
    }

    [HttpPost("/register")]
    public IActionResult Register()
    {
        return Ok();
    }
}
