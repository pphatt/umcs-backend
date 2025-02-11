using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.TestApi;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("auth")]
    [Authorize]
    [Authorize(Permissions.Users.View)]
    public IActionResult TestAuth()
    {
        return Ok("Access auth route successfully.");
    }
}
