using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.FacultyApp.Commands.CreateFaculty;
using Server.Contracts.Faculties.CreateFaculty;
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
    public async Task<IActionResult> CreateFaculties([FromForm] CreateFacultyRequest request)
    {
        var mapper = _mapper.Map<CreateFacultyCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }
}
