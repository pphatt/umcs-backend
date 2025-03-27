﻿using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Extensions;
using Server.Application.Features.Notification.Commands.MarkAllNotificationsAsRed;
using Server.Application.Features.Notification.Commands.MarkNotificationAsRed;
using Server.Application.Features.Notification.Commands.UnreadNotification;
using Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;
using Server.Contracts.Notifications.GetAllUserNotificationsPagination;
using Server.Contracts.Notifications.MarkAllNotificationsAsRed;
using Server.Contracts.Notifications.MarkNotificationAsRed;
using Server.Contracts.Notifications.UnreadNotification;

namespace Server.Api.Controllers.ClientApi;

public class NotificationsController : ClientApiController
{
    private readonly IMapper _mapper;

    public NotificationsController(IMapper mapper, ISender mediatorSender) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetAllUsersNotificationsPagination([FromQuery] GetAllUserNotificationsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllUserNotificationsPaginationQuery>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("has-red")]
    public async Task<IActionResult> HasRedNotification(MarkNotificationAsRedRequest request)
    {
        var mapper = _mapper.Map<MarkNotificationAsRedCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            redResult => Ok(redResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("has-red-all")]
    public async Task<IActionResult> HasRedAllNotifications(MarkAllNotificationsAsRedRequest request)
    {
        var mapper = _mapper.Map<MarkAllNotificationsAsRedCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            redResult => Ok(redResult),
            errors => Problem(errors)
        );
    }

    [HttpPost("un-read")]
    public async Task<IActionResult> UnreadNotification(UnreadNotificationRequest request)
    {
        var mapper = _mapper.Map<UnreadNotificationCommand>(request);

        mapper.UserId = User.GetUserId();

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            unreadResult => Ok(unreadResult),
            errors => Problem(errors)
        );
    }
}
