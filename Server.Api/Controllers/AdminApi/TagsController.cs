using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.TagApp.Commands.BulkDeleteTags;
using Server.Application.Features.TagApp.Commands.CreateTag;
using Server.Application.Features.TagApp.Commands.DeleteTag;
using Server.Application.Features.TagApp.Commands.UpdateTag;
using Server.Application.Features.TagApp.Queries.GetAllTagsPagination;
using Server.Application.Features.TagApp.Queries.GetTagById;
using Server.Contracts.Tags.BulkDeleteTags;
using Server.Contracts.Tags.CreateTag;
using Server.Contracts.Tags.DeleteTag;
using Server.Contracts.Tags.GetAllTagsPagination;
using Server.Contracts.Tags.GetTagById;
using Server.Contracts.Tags.UpdateTag;
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

    [HttpDelete("bulk-delete")]
    [Authorize(Permissions.Tags.Delete)]
    public async Task<IActionResult> BulkDeleteTags(BulkDeleteTagsRequest request)
    {
        var mapper = _mapper.Map<BulkDeleteTagsCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            deleteResult => Ok(deleteResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("{Id}")]
    [Authorize(Permissions.Tags.View)]
    public async Task<IActionResult> GetTagById([FromRoute] GetTagByIdRequest request)
    {
        var mapper = _mapper.Map<GetTagByIdQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("pagination")]
    [Authorize(Permissions.Tags.View)]
    public async Task<IActionResult> GetAllTagsPagination([FromQuery] GetAllTagsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllTagsPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
