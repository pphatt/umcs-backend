using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Common.Extensions;
using Server.Application.Features.ContributionApp.Queries.GetAllContributionsPagination;
using Server.Application.Features.ContributionApp.Queries.GetPersonalContributionDetailBySlug;
using Server.Application.Features.Identity.Commands.ForgotPassword;
using Server.Application.Features.Identity.Commands.ResetPassword;
using Server.Application.Features.Identity.Commands.ValidateForgotPasswordToken;
using Server.Application.Features.PublicContributionApp.Queries.GetAllBookmarkPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetAllReadLaterPagination;
using Server.Application.Features.PublicContributionApp.Queries.GetAllUserLikePublicContributionsPagination;
using Server.Contracts.Contributions.CoordinatorGetAllContributionsPagination;
using Server.Contracts.Contributions.GetPersonalContributionDetailBySlug;
using Server.Contracts.Identity.ForgotPassword;
using Server.Contracts.Identity.ResetPassword;
using Server.Contracts.Identity.ValidateForgotPasswordToken;
using Server.Contracts.PublicContributions.GetAllBookmarkPagination;
using Server.Contracts.PublicContributions.GetAllReadLaterPagination;
using Server.Contracts.PublicContributions.GetAllUserLikePublicContributionsPagination;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.ClientApi;

public class UsersController : ClientApiController
{
    private readonly IMapper _mapper;

    public UsersController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var mapper = _mapper.Map<ForgotPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            forgotPasswordResult => Ok(forgotPasswordResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var mapper = _mapper.Map<ResetPasswordCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            resetPasswordResult => Ok(resetPasswordResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("validate-forgot-password-token")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateForgotPasswordToken(ValidateForgotPasswordTokenRequest request)
    {
        var mapper = _mapper.Map<ValidateForgotPasswordTokenCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            validateResult => Ok(validateResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("recent-contributions")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetRecentContribution(GetAllContributionsPaginationRequest request)
    {
        /* This is personal contribution pagination */

        var mapper = _mapper.Map<GetAllContributionsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();
        mapper.FacultyName = User.GetUserFacultyName();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("preview-contribution/{Slug}")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetPersonalContributionDetailBySlug([FromRoute] GetPersonalContributionDetailBySlugRequest request)
    {
        /*
         * This contribution view flow is designed to streamline the front-end (FE) implementation by clarifying the API usage based on the contribution's state.
         * Here's how it works:
         *
         * - When a user (the owner) accesses their "recent-contributions" (a list of all contributions they've made) and clicks on one:
         *   - There are two possible scenarios:
         *     1. The contribution is not yet public (e.g., pending or rejected).
         *     2. The contribution is already a public contribution (approved).
         *
         * - Scenario 1 (Not Public Yet or Rejected):
         *   - The FE will use the endpoint: "client-api/user-controller/preview-contribution/{slug}"
         *   - This retrieves data directly from the "Contributions" table, allowing only the owner to view their unpublished or rejected work.
         *
         * - Scenario 2 (Public Contribution):
         *   - The FE will use the endpoint: "client-api/public-contribution-controller/contribution/{slug}"
         *   - This fetches data from the "PublicContributions" table, reflecting the approved and publicly available contribution.
         *
         * - Front-End Logic:
         *   - The "recent-contributions" list returned to the FE includes metadata like `PublicDate` (or an `IsPublicYet` flag).
         *   - This metadata determines which API route to call next:
         *     - If `PublicDate` is null (or `IsPublicYet` is false), use the user-specific endpoint.
         *     - If `PublicDate` exists (or `IsPublicYet` is true), use the public endpoint.
         */

        /*
         * My friend's approach is that:
         * - Public contributions use "client-api/public-contribution-controller/contribution/{slug}" from "PublicContributions" table.
         * - Ungraded contributions use "coordinator-api/contribution-controller/preview-contribution/{slug}" from "Contribution" table.
         * - The same "client-api/user-controller/contribution/{slug}" endpoint also handles updates.
         */

        /*
         * The different from two approaches is that when the private contribution get requested my approach will use the "client-api/user-controller/preview-contribution/{slug}"
         * but other will use "coordinator-api/contribution-controller/preview-contribution/{slug}".
         */

        var mapper = _mapper.Map<GetPersonalContributionDetailBySlugQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("like-contributions-pagination")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllUserLikePublicContributionsPagination([FromQuery] GetAllUserLikePublicContributionsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllUserLikePublicContributionsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("read-later")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllReadLaterPagination([FromQuery] GetAllReadLaterPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllReadLaterPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("bookmark-pagination")]
    [Authorize(Permissions.Contributions.View)]
    public async Task<IActionResult> GetAllBookmarkPagination([FromQuery] GetAllBookmarkPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllBookmarkPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
