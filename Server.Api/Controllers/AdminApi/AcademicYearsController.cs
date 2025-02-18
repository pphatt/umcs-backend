using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Contracts.AcademicYears.CreateAcademicYear;
using System.Runtime.InteropServices;

namespace Server.Api.Controllers.AdminApi;

public class AcademicYearsController : AdminApiController
{
    private readonly IMapper _mapper;

    public AcademicYearsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAcademicYear([FromForm] CreateAcademicYearRequest request)
    {
        var mapper = _mapper.Map<CreateAcademicYearCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }
}
