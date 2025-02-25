using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Queries.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;
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
}
