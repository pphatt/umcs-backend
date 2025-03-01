using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionsPagination;
using Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.ClientApi;

[Tags("Contributions Student")]
public class PublicContributionsController : ClientApiController
{
    private readonly IMapper _mapper;

    public PublicContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllPublicContributionPagination([FromQuery] GetAllPublicContributionsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllPublicContributionsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
