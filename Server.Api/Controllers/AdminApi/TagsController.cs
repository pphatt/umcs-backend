using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.ContributionTagApp.Commands.CreateTag;
using Server.Contracts.ContributionTags.CreateTag;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

[Tags("Tags")]
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
}
