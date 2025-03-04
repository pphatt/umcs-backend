using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.PublicContributionApp.Commands.ToggleLikeContribution;
using Server.Application.Features.PublicContributionApp.Queries.DownloadAllFiles;
using Server.Application.Features.PublicContributionApp.Queries.DownloadSingleFile;
using Server.Application.Features.PublicContributionApp.Queries.GetAllPublicContributionsPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetLatestPublicContributions;
using Server.Application.Features.PublicContributionApp.Queries.GetListUserLiked;
using Server.Application.Features.PublicContributionApp.Queries.GetPublicContributionBySlug;
using Server.Application.Features.PublicContributionApp.Queries.GetTopMostLikedPublicContributions;
using Server.Application.Features.PublicContributionApp.Queries.GetTopMostViewedPublicContributions;
using Server.Application.Features.PublicContributionCommentApp.Commands;
using Server.Contracts.PublicContributionComments.CreatePublicComment;
using Server.Contracts.PublicContributions.DownloadAllFiles;
using Server.Contracts.PublicContributions.DownloadSingleFile;
using Server.Contracts.PublicContributions.GetAllPublicContributionsPagination;
using Server.Contracts.PublicContributions.GetAllUsersLikedContributionPagination;
using Server.Contracts.PublicContributions.GetLatestPublicContributions;
using Server.Contracts.PublicContributions.GetPublicContributionBySlug;
using Server.Contracts.PublicContributions.GetTopMostLikedPublicContributions;
using Server.Contracts.PublicContributions.GetTopMostViewedPublicContributions;
using Server.Contracts.PublicContributions.ToggleLikeContribution;
using Server.Domain.Common.Constants.Authorization;
using Server.Domain.Common.Constants.Content;

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

    [HttpGet("guest/pagination")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllGuestContributionPagination([FromQuery] GetAllPublicContributionsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllPublicContributionsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.AllowedGuest = true;

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("contribution/{Slug}")]
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

    [HttpGet("download-files/{ContributionId}")]
    [Authorize(Permissions.Contributions.Download)]
    public async Task<IActionResult> DownloadAllFiles([FromRoute] DownloadAllFilesRequests request)
    {
        var mapper = _mapper.Map<DownloadAllFilesQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            downloadResult => Ok(downloadResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("toggle-like/{ContributionId}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> ToggleLikeContribution([FromRoute] ToggleLikeContributionRequest request)
    {
        var mapper = _mapper.Map<ToggleLikeContributionCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            toggleLikeResult => Ok(toggleLikeResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("who-liked/{ContributionId}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllUsersLikedContributionPagination(GetAllUsersLikedContributionPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllUsersLikedContributionPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("comment/{ContributionId}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> CreatePublicComment(CreatePublicCommentRequest request)
    {
        var mapper = _mapper.Map<CreatePublicCommentCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            commentResult => Ok(commentResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("latest")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetLatestContribution([FromQuery] GetLatestPublicContributionsRequest request)
    {
        var mapper = _mapper.Map<GetLatestPublicContributionsQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.SortBy = ContributionSortBy.PublicDate.ToStringValue();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("top-most-liked-contributions")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetTopMostLikedPublicContributions([FromQuery] GetTopMostLikedPublicContributionsRequest request)
    {
        var mapper = _mapper.Map<GetTopMostLikedPublicContributionsQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.SortBy = ContributionSortBy.Like.ToStringValue();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("top-most-viewed-contributions")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetTopMostViewedPublicContributions([FromQuery] GetTopMostViewedPublicContributionsRequest request)
    {
        var mapper = _mapper.Map<GetTopMostViewedPublicContributionsQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.SortBy = ContributionSortBy.View.ToStringValue();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
