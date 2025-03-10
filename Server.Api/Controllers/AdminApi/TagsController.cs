using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.ContributionTagApp.Commands.CreateTag;
using Server.Application.Features.ContributionTagApp.Commands.DeleteTag;
using Server.Application.Features.ContributionTagApp.Commands.UpdateTag;
using Server.Contracts.ContributionTags.CreateTag;
using Server.Contracts.ContributionTags.DeleteTag;
using Server.Contracts.ContributionTags.UpdateTag;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

public class TagsController : AdminApiController
{
    private readonly IMapper _mapper;

    public TagsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [Authorize(Permissions.Tags.Create)]
    public async Task<IActionResult> CreateNewTag([FromForm] CreateTagRequest request)
    {
        var mapper = _mapper.Map<CreateTagCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("{Id}")]
    [Authorize(Permissions.Tags.Edit)]
    public async Task<IActionResult> UpdateTag([FromRoute] UpdateTagRequest request)
    {
        var mapper = _mapper.Map<UpdateTagCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{Id}")]
    [Authorize(Permissions.Tags.Delete)]
    public async Task<IActionResult> DeleteTag([FromRoute] DeleteTagRequest request)
    {
        var mapper = _mapper.Map<DeleteTagCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            deleteResult => Ok(deleteResult),
            errors => Problem(errors)
        );
    }
}
