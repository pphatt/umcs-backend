using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.ContributionActivityLogsApp.Queries.GetAllContributionActivityLogsPagination;
using Server.Contracts.ContributionActivityLogs.GetAllContributionActivityLogsPagination;
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
}
