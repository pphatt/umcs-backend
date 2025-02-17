using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Identity.Commands.DeleteUser;
using Server.Application.Features.Identity.Commands.UpdateUser;
using Server.Application.Features.Identity.Queries.GetAllUsersPagination;
using Server.Application.Features.Identity.Queries.GetUserById;
using Server.Application.Features.Users.Commands.CreateUser;
using Server.Contracts.Identity.CreateUser;
using Server.Contracts.Identity.DeleteUser;
using Server.Contracts.Identity.GetAllUsersPagination;
using Server.Contracts.Identity.GetUserById;
using Server.Contracts.Identity.UpdateUser;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

public class UsersController : AdminApiController
{
    private readonly IMapper _mapper;

    public UsersController(IMapper mapper, ISender mediatorSender) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [Authorize(Permissions.Users.Create)]
    public async Task<IActionResult> CreateUser([FromForm] CreateUserRequest request)
    {
        var mapper = _mapper.Map<CreateUserCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("{Id}")]
    [Authorize(Permissions.Users.Edit)]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        var mapper = _mapper.Map<UpdateUserCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{Id}")]
    [Authorize(Permissions.Users.Delete)]
    public async Task<IActionResult> DeleteUser([FromRoute] DeleteUserRequest request)
    {
        var mapper = _mapper.Map<DeleteUserCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            deleteResult => Ok(deleteResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("{Id}")]
    [Authorize(Permissions.Users.View)]
    public async Task<IActionResult> GetUserById([FromRoute] GetUserByIdRequest request)
    {
        var mapper = _mapper.Map<GetUserByIdQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            getByIdResult => Ok(getByIdResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("pagination")]
    [Authorize(Permissions.Users.View)]
    public async Task<IActionResult> GetAllUsersPagination([FromQuery] GetAllUsersPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllUsersPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
