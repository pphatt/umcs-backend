using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Authentication.Commands.Login;
using Server.Contracts.Authentication;

namespace Server.Api.Controllers;

[ApiController]
[Route("/authentication")]
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
    [Route("register")]
    public IActionResult Register()
    {
        return Ok();
    }
}
