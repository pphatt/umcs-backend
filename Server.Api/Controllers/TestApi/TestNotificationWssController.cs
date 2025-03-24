using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Application.Common.Extensions;
using Server.Application.Features.Notification.Queries.GetAllUserNotificationsPagination;
using Server.Contracts.Notifications.GetAllUserNotificationsPagination;

namespace Server.Api.Controllers.TestApi;

[Authorize]
public class TestNotificationWssController : TestApiController
{
    private readonly ISender _mediatorSender;
    private readonly IMapper _mapper;

    public TestNotificationWssController(ISender mediatorSender, IMapper mapper)
    {
        _mediatorSender = mediatorSender;
        _mapper = mapper;
    }

    [HttpGet("/pagination")]
    public async Task<IActionResult> GetAllUserNotificationsPagination([FromQuery] GetAllUserNotificationsPaginationRequest request)
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
