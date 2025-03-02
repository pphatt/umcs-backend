using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;
using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;
using Server.Contracts.Contributions.GetPersonalContributionDetailBySlug;
using Server.Contracts.Identity.ForgotPassword;
using Server.Contracts.Identity.ResetPassword;
using Server.Contracts.Identity.ValidateForgotPasswordToken;
using Server.Domain.Common.Constants.Authorization;
using System.ComponentModel;

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

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var mapper = _mapper.Map<ResetPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            resetPasswordResult => Ok(resetPasswordResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("validate-forgot-password-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateForgotPasswordToken(ValidateForgotPasswordTokenRequest request)
    {
        var mapper = _mapper.Map<ValidateForgotPasswordTokenCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            validateResult => Ok(validateResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("contribution/{Slug}")]
    [Description("View contribution here is split into two parts: if the contribution is not yet public or rejection, it will be retrieved from the contribution itself for only the student (the owner) to see; otherwise, it will come from the public contribution once approved.")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetPersonalContributionDetailBySlug([FromRoute] GetPersonalContributionDetailBySlugRequest request)
    {
        var mapper = _mapper.Map<GetPersonalContributionDetailBySlugQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
