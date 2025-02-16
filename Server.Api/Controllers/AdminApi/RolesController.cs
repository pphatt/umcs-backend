using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.Role.Commands.CreateRole;
using Server.Application.Features.Role.Commands.DeleteRole;
using Server.Application.Features.Role.Commands.UpdateRole;
using Server.Application.Features.Role.Queries.GetAllRolePermissions;
using Server.Application.Features.Role.Queries.GetAllRolesPagination;
using Server.Application.Features.Role.Queries.GetRoleById;
using Server.Contracts.Roles.CreateRole;
using Server.Contracts.Roles.DeleteRole;
using Server.Contracts.Roles.GetAllRolePermissions;
using Server.Contracts.Roles.GetAllRolesPagination;
using Server.Contracts.Roles.GetRoleById;
using Server.Contracts.Roles.UpdateRole;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

public class RolesController : AdminApiController
{
    private readonly IMapper _mapper;

    public RolesController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [Authorize(Permissions.Roles.Create)]
    public async Task<IActionResult> CreateRole([FromForm] CreateRoleRequest request)
    {
        var mapper = _mapper.Map<CreateRoleCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("update")]
    [Authorize(Permissions.Roles.Edit)]
    public async Task<IActionResult> EditRole([FromForm] UpdateRoleRequest request)
    {
        var mapper = _mapper.Map<UpdateRoleCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }

    [HttpDelete("delete/{Id}")]
    [Authorize(Permissions.Roles.Delete)]
    public async Task<IActionResult> DeleteRole([FromRoute] DeleteRoleRequest request)
    {
        var mapper = _mapper.Map<DeleteRoleCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            deleteResult => Ok(deleteResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("{Id}")]
    [Authorize(Permissions.Roles.View)]
    public async Task<IActionResult> GetRoleById([FromRoute] GetRoleByIdRequest request)
    {
        var mapper = _mapper.Map<GetRoleByIdQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet]
    [Authorize(Permissions.Roles.View)]
    public async Task<IActionResult> GetAllRolesPagination([FromQuery] GetAllRolesPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllRolesPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }

    [HttpGet("{Id}/permissions")]
    [Authorize(Permissions.Roles.View)]
    public async Task<IActionResult> GetAllRolePermissions([FromRoute] GetAllRolePermissionsRequest request)
    {
        var mapper = _mapper.Map<GetAllRolePermissionsQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            queryResult => Ok(queryResult),
            errors => Problem(errors)
        );
    }
}
