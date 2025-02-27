using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.PublicContributionApp.Commands.AllowGuest;
using Server.Contracts.PublicContributions.AllowGuest;
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

        mapper.facultyId = User.GetUserFacultyId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            allowResult => Ok(allowResult),
            errors => Problem(errors)
        );
    }
}
