using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.AcademicYearsApp.Commands.CreateAcademicYear;
using Server.Application.Features.AcademicYearsApp.Commands.UpdateAcademicYear;
using Server.Contracts.AcademicYears.CreateAcademicYear;
using Server.Contracts.AcademicYears.UpdateAcademicYear;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

public class AcademicYearsController : AdminApiController
{
    private readonly IMapper _mapper;

    public AcademicYearsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpPost("create")]
    [Authorize(Permissions.AcademicYears.Create)]
    public async Task<IActionResult> CreateAcademicYear([FromForm] CreateAcademicYearRequest request)
    {
        var mapper = _mapper.Map<CreateAcademicYearCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            createResult => Ok(createResult),
            errors => Problem(errors)
        );
    }

    [HttpPut("{Id}")]
    [Authorize(Permissions.AcademicYears.Edit)]
    public async Task<IActionResult> UpdateAcademicYear(UpdateAcademicYearRequest request)
    {
        var mapper = _mapper.Map<UpdateAcademicYearCommand>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            updateResult => Ok(updateResult),
            errors => Problem(errors)
        );
    }
}
