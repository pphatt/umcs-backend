using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestAuthController : TestApiController
{
    [HttpGet("auth")]
    [Authorize]
    [Authorize(Permissions.Users.View)]
    public IActionResult TestAuth()
    {
        return Ok("Access auth route successfully.");
    }
}
