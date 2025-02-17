using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Application.Features.FacultyApp.Commands.DeleteFaculty;
using Server.Application.Features.FacultyApp.Commands.UpdateFaculty;
using Server.Contracts.Faculties.CreateFaculty;
using Server.Contracts.Faculties.DeleteFaculty;
using Server.Contracts.Faculties.UpdateFaculty;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

public class FacultiesController : AdminApiController
{
    private readonly IMapper _mapper;

    public FacultiesController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [Authorize(Permissions.Faculties.Create)]
    public async Task<IActionResult> CreateFaculty([FromForm] CreateFacultyRequest request)
    {
        var mapper = _mapper.Map<CreateFacultyCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("{Id}")]
    [Authorize(Permissions.Faculties.Edit)]
    public async Task<IActionResult> UpdateFaculty(UpdateFacultyRequest request)
    {
        var mapper = _mapper.Map<UpdateFacultyCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{Id}")]
    [Authorize(Permissions.Faculties.Delete)]
    public async Task<IActionResult> DeleteFaculty([FromRoute] DeleteFacultyRequest request)
    {
        var mapper = _mapper.Map<DeleteFacultyCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            deleteResult => Ok(deleteResult),
            errors => Problem(errors)
        );
    }
}
