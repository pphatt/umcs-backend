using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.PublicContributionApp.Commands.AllowGuest;
using Server.Application.Features.PublicContributionApp.Commands.AllowGuestWithManyContributions;
using Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuest;
using Server.Application.Features.PublicContributionApp.Commands.RevokeAllowGuestWithManyContributions;
using Server.Contracts.PublicContributions.AllowGuest;
using Server.Contracts.PublicContributions.AllowGuestWithManyContributions;
using Server.Contracts.PublicContributions.RevokeAllowGuest;
using Server.Contracts.PublicContributions.RevokeAllowGuestWithManyContributions;
using Server.Domain.Common.Constants.Authorization;
using System.ComponentModel;

namespace Server.Api.Controllers.CoordinatorApi;

[Tags("Contributions Coordinator")]
public class PublicContributionsController : CoordinatorApiController
{
    private readonly IMapper _mapper;

    public PublicContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("allow-guest")]
    [Description("Allowed both contribution and public contribution to guest but only query the public contribution for guest when paging.")]
    [Authorize(Permissions.SettingGAC.Manage)]
    public async Task<IActionResult> AllowGuest(AllowGuestRequest request)
    {
        var mapper = _mapper.Map<AllowGuestCommand>(request);

        mapper.FacultyId = User.GetUserFacultyId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            allowResult => Ok(allowResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("revoke-allow-guest")]
    [Description("Remove guest permission to view the contribution.")]
    [Authorize(Permissions.SettingGAC.Manage)]
    public async Task<IActionResult> RevokeAllowGuest(RevokeAllowGuestRequest request)
    {
        var mapper = _mapper.Map<RevokeAllowGuestCommand>(request);

        mapper.FacultyId = User.GetUserFacultyId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            revokeAllowResult => Ok(revokeAllowResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("allow-guest-with-many-contributions")]
    [Authorize(Permissions.SettingGAC.Manage)]
    public async Task<IActionResult> AllowGuestWithManyContributions(AllowGuestWithManyContributionsRequest request)
    {
        var mapper = _mapper.Map<AllowGuestWithManyContributionsCommand>(request);

        mapper.FacultyId = User.GetUserFacultyId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            allowResult => Ok(allowResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("revoke-allow-guest-with-many-contributions")]
    [Authorize(Permissions.SettingGAC.Manage)]
    public async Task<IActionResult> RevokeAllowGuestWithManyContributions(RevokeAllowGuestWithManyContributionsRequest request)
    {
        var mapper = _mapper.Map<RevokeAllowGuestWithManyContributionsCommand>(request);

        mapper.FacultyId = User.GetUserFacultyId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            revokeResult => Ok(revokeResult),
            errors => Problem(errors)
        );
    }
}
