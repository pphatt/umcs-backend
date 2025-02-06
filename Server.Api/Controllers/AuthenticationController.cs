using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Contracts.Authentication;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;

namespace Server.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    IMediator _mediator;
    IMapper _mapper;

    public AuthenticationController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var mapper = _mapper.Map<LoginCommand>(request);

        var response = await _mediator.Send(mapper);

        return Ok(new AuthenticationResponse(response.AccessToken, response.RefreshToken));
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var mapper = _mapper.Map<RefreshTokenCommand>(request);

        var response = await _mediator.Send(mapper);

        return Ok(new AuthenticationResponse(response.AccessToken, response.RefreshToken));
    }

    [HttpGet]
    [Authorize]
    [Route("test-auth")]
    public IActionResult TestAuthRoute()
    {
        return Ok("Access auth route successfully.");
    }
}
