using Microsoft.AspNetCore.Mvc;

namespace Server.Api.Controllers.TestApi;

[Tags("Test")]
public class TestApiControllerDockerSample : TestApiController
{
    [HttpGet("test-docker-compose-up")]
    public IActionResult TestDockerComposeUp()
    {
        return Ok("Docker compose up successfully.");
    }

    [HttpGet("test-docker-compose-up-build")]
    public IActionResult TestDockerComposeUpBuild()
    {
        return Ok("Docker compose up successfully.");
    }
}
