using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.PublicContributionApp.Queries.DownloadSingleFile;
using Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionsPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;
using Server.Contracts.PublicContributions.DownloadSingleFile;
using Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;
using Server.Contracts.PublicContributions.GetPublicContributionBySlug;
using Server.Domain.Common.Constants.Authorization;
using System.Runtime.InteropServices;

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

    [HttpGet("{Slug}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetPublicContributionBySlug([FromRoute] GetPublicContributionBySlugRequest request)
    {
        var mapper = _mapper.Map<GetPublicContributionBySlugQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("download-file")]
    [Authorize(Permissions.Contributions.Download)]
    public async Task<IActionResult> DownloadSingleFile([FromQuery] DownloadSingleFileRequest request)
    {
        var mapper = _mapper.Map<DownloadSingleFileQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            downloadResult => Ok(downloadResult),
            errors => Problem(errors)
        );
    }
}
