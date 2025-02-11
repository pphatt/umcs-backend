using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Application.Features.Authentication.Commands.RefreshToken;
using Server.Contracts.Authentication;
using Server.Contracts.Authentication.Login;
using Server.Contracts.Authentication.RefreshToken;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.Authentication;

[Route("api/auth/[controller]")]
public class AuthenticationController : ApiController
{
    private readonly IMapper _mapper;

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
            loginResult => Ok(loginResult),
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
            refreshTokenResult => Ok(refreshTokenResult),
            errors => Problem(errors)
        );
    }
}
