using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Extensions;
using Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;
using Server.Contracts.Notifications.GetAllUserNotificationsPagination;

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
}
