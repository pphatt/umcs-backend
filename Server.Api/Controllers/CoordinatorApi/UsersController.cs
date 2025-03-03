using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Identity.Commands.CreateGuest;
using Server.Contracts.Identity.CreateGuest;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.CoordinatorApi;

public class UsersController : CoordinatorApiController
{
    private readonly IMapper _mapper;

    public UsersController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create-guest")]
    [Authorize(Permissions.Guest.Create)]
    public async Task<IActionResult> CreateGuestUser(CreateGuestRequest request)
    {
        var mapper = _mapper.Map<CreateGuestCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }
}
