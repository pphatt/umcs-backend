using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Commands.ApproveContribution;
using Server.Application.Features.ContributionApp.Commands.RejectContribution;
using Server.Application.Features.ContributionApp.Queries.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.ApproveContribution;
using Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.RejectContribution;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.CoordinatorApi;

public class ContributionsController : CoordinatorApiController
{
    private readonly IMapper _mapper;

    public ContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    [Authorize(Permissions.ManageContributions.Manage)]
    public async Task<IActionResult> GetAllContributionsPagination([FromQuery] CoordinatorGetAllContributionsPaginationRequest request)
    {
        var mapper = _mapper.Map<CoordinatorGetAllContributionsPaginationQuery>(request);

        mapper.Faculty = User.GetUserFacultyName();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("approve")]
    [Authorize(Permissions.Contributions.Approve)]
    public async Task<IActionResult> ApproveContribution(ApproveContributionRequest request)
    {
        var mapper = _mapper.Map<ApproveContributionCommand>(request);

        mapper.CoordinatorId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            approveResult => Ok(approveResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("reject")]
    [Authorize(Permissions.Contributions.Reject)]
    public async Task<IActionResult> RejectContribution(RejectContributionRequest request)
    {
        var mapper = _mapper.Map<RejectContributionCommand>(request);

        mapper.CoordinatorId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            rejectResult => Ok(rejectResult),
            errors => Problem(errors)
        );
    }
}
