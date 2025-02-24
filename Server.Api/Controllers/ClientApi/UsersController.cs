using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Identity.Commands.ForgotPassword;

namespace Server.Api.Controllers.ClientApi;

public class UsersController : ClientApiController
{
    private readonly IMapper _mapper;

    public UsersController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var mapper = _mapper.Map<ForgotPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            forgotPasswordResult => Ok(forgotPasswordResult),
            errors => Problem(errors)
        );
    }
}
