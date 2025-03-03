using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Commands.ApproveContribution;
using Server.Application.Features.ContributionApp.Commands.RejectContribution;
using Server.Application.Features.ContributionApp.Queries.GetAllContributionsPagination;
using Server.Application.Features.ContributionApp.Queries.GetContributionBySlug;
using Server.Application.Features.ContributionCommentApp.Commands.CreateComment;
using Server.Contracts.ContributionComments.CreateComment;
using Server.Contracts.Contributions.ApproveContribution;
using Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.GetContributionBySlug;
using Server.Contracts.Contributions.RejectContribution;
using Server.Domain.Common.Constants.Authorization;
using System.ComponentModel;

namespace Server.Api.Controllers.CoordinatorApi;

[Tags("Contributions Coordinator")]
public class ContributionsController : CoordinatorApiController
{
    private readonly IMapper _mapper;

    public ContributionsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    [Authorize(Permissions.ManageContributions.Manage)]
    public async Task<IActionResult> GetAllContributionsPagination([FromQuery] GetAllContributionsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllContributionsPaginationQuery>(request);

        mapper.Faculty = User.GetUserFacultyName();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("approve")]
    [Authorize(Permissions.Contributions.Approve)]
    public async Task<IActionResult> ApproveContribution(ApproveContributionRequest request)
    {
        var mapper = _mapper.Map<ApproveContributionCommand>(request);

        mapper.CoordinatorId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            approveResult => Ok(approveResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("reject")]
    [Authorize(Permissions.Contributions.Reject)]
    public async Task<IActionResult> RejectContribution(RejectContributionRequest request)
    {
        var mapper = _mapper.Map<RejectContributionCommand>(request);

        mapper.CoordinatorId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            rejectResult => Ok(rejectResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("preview-contribution/{Slug}")]
    [Description("Preview contribution here is when contribution is not yet to be approved or rejected, still at pending state so only the student (contribution's owner) and faculty coordinator can view.")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> PreviewContribution([FromRoute] GetContributionBySlugRequest request)
    {
        var mapper = _mapper.Map<GetContributionBySlugQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.FacultyName = User.GetUserFacultyName();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("comment/{ContributionId}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> CreateContributionComment(CreateCommentRequest request)
    {
        var mapper = _mapper.Map<CreateCommentCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }
}
