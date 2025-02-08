using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Contracts.Authentication;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;

namespace Server.Api.Controllers.Authentication;

[Route("[controller]")]
public class AuthenticationController : ApiController
{
    IMapper _mapper;

    public AuthenticationController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var mapper = _mapper.Map<LoginCommand>(request);

        var response = await _mediatorSender.Send(mapper);

        return response.Match(
            loginResult => Ok(new AuthenticationResponse(loginResult.AccessToken, loginResult.RefreshToken)),
            errors => Problem(errors)
        );
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var mapper = _mapper.Map<RefreshTokenCommand>(request);

        var response = await _mediatorSender.Send(mapper);

        return response.Match(
            refreshTokenResult => Ok(new AuthenticationResponse(refreshTokenResult.AccessToken, refreshTokenResult.RefreshToken)),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [Authorize]
    [Route("test-auth")]
    public IActionResult TestAuthRoute()
    {
        return Ok("Access auth route successfully.");
    }
}
