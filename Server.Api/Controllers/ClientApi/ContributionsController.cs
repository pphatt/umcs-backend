using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Api.Common.Filters;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Commands.CreateContribution;
using Server.Application.Features.ContributionApp.Commands.UpdateContribution;
using Server.Contracts.Contributions.CreateContribution;
using Server.Contracts.Contributions.UpdateContribution;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.ClientApi;

public class ContributionsController : ClientApiController
{
    private readonly IMapper _mapper;

    public ContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [FileValidationFilter(5 * 1024 * 1024)]
    [Authorize(Permissions.Contributions.Create)]
    public async Task<IActionResult> CreateContribution([FromForm] CreateContributionRequest request)
    {
        var mapper = _mapper.Map<CreateContributionCommand>(request);

        mapper.UserId = User.GetUserId();
        mapper.FacultyId = User.GetUserFacultyId();
        mapper.Slug = request.Title.Slugify();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("{Id}")]
    [FileValidationFilter(5 * 1024 * 1024)]
    [Authorize(Permissions.Contributions.Edit)]
    public async Task<IActionResult> UpdateContribution([FromRoute] Guid Id, [FromForm] UpdateContributionRequest request)
    {
        var mapper = _mapper.Map<UpdateContributionCommand>(request);

        mapper.Id = Id;
        mapper.UserId = User.GetUserId();
        mapper.FacultyId = User.GetUserFacultyId();
        mapper.Slug = request.Title.Slugify();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }
}
