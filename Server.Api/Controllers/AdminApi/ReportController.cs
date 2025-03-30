using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyInEachAcademicYear;
using Server.Domain.Common.Constants.Authorization;

namespace Server.Api.Controllers.AdminApi;

[Tags("Admin Reports")]
public class ReportController : AdminApiController
{
    private readonly IMapper _mapper;

    public ReportController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("get-total-contributions-in-each-faculty-in-each-academic-year")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetTotalContributionsInEachFacultyInEachAcademicYear()
    {
        var command = new GetTotalContributionsInEachFacultyInEachAcademicYearQuery();

        var result = await _mediatorSender.Send(command);

        return Ok(result);
    }
}
