using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetAllContributionActivityLogsPagination;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogById;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetContributionActivityLogsByContributionId;
using Server.Contracts.ContributionActivityLogs.GetAllContributionActivityLogsPagination;
using Server.Contracts.ContributionActivityLogs.GetContributionActivityLogById;
using Server.Contracts.ContributionActivityLogs.GetContributionActivityLogsByContributionId;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.ManagerApi;

public class ContributionsController : ManagerApiController
{
    private readonly IMapper _mapper;

    public ContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("activity-logs/pagination")]
    [Authorize(Permissions.ActivityLogs.View)]
    public async Task<IActionResult> GetAllContributionActivityLogsPagination(GetAllContributionActivityLogsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllContributionActivityLogsPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("activity-logs/{Id}")]
    [Authorize(Permissions.ActivityLogs.View)]
    public async Task<IActionResult> GetContributionActivityLogById([FromRoute] GetContributionActivityLogByIdRequest request)
    {
        var mapper = _mapper.Map<GetContributionActivityLogByIdQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("activity-logs/contribution/{Id}")]
    [Authorize(Permissions.ActivityLogs.View)]
    public async Task<IActionResult> GetActivityLogByContributionId([FromRoute] GetContributionActivityLogsByContributionIdRequest request)
    {
        var mapper = _mapper.Map<GetContributionActivityLogsByContributionIdQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
