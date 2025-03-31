using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;
using Server.Application.Features.Report.Queries.GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear;
using Server.Application.Features.Report.Queries.GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYear;
using Server.Application.Features.Report.Queries.GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYear;
using Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyForAnyAcademicYear;
using Server.Application.Features.Report.Queries.GetTotalContributionsInEachFacultyInEachAcademicYear;
using Server.Application.Features.Report.Queries.GetTotalContributorsByEachFacultyForAnyAcademicYear;
using Server.Application.Features.Report.Queries.GetTotalContributorsInEachFacultyInEachAcademicYear;
using Server.Contracts.Report.GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear;
using Server.Contracts.Report.GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYear;
using Server.Contracts.Report.GetTotalContributionsInEachFacultyForAnyAcademicYear;
using Server.Contracts.Report.GetTotalContributorsByEachFacultyForAnyAcademicYear;
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
        var query = new GetTotalContributionsInEachFacultyInEachAcademicYearQuery();

        var result = await _mediatorSender.Send(query);

        return Ok(result);
    }

    [HttpGet("get-total-contributions-in-each-faculty-for-any-academic-year/{AcademicYearName}")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetTotalContributionsInEachFacultyForAnyAcademicYear([FromRoute] GetTotalContributionsInEachFacultyForAnyAcademicYearRequest request)
    {
        var mapper = _mapper.Map<GetTotalContributionsInEachFacultyForAnyAcademicYearQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return Ok(result);
    }

    [HttpGet("get-percentage-of-total-contributions-in-each-faculty-in-each-academic-year")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYear()
    {
        var query = new GetPercentageOfTotalContributionsInEachFacultyInEachAcademicYearQuery();

        var result = await _mediatorSender.Send(query);

        return Ok(result);
    }

    [HttpGet("get-percentage-of-total-contributions-by-each-faculty-for-any-academic-year/{AcademicYearName}")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYear([FromRoute] GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearRequest request)
    {
        var mapper = _mapper.Map<GetPercentageOfTotalContributionsByEachFacultyForAnyAcademicYearQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return Ok(result);
    }

    [HttpGet("get-total-contributors-in-each-faculty-in-each-academic-year")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetTotalContributorsInEachFacultyInEachAcademicYear()
    {
        var query = new GetTotalContributorsInEachFacultyInEachAcademicYearQuery();

        var result = await _mediatorSender.Send(query);

        return Ok(result);
    }

    [HttpGet("get-total-contributors-by-each-faculty-for-any-academic-year/{AcademicYearName}")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetTotalContributorsByEachFacultyForAnyAcademicYear([FromRoute] GetTotalContributorsByEachFacultyForAnyAcademicYearRequest request)
    {
        var mapper = _mapper.Map<GetTotalContributorsByEachFacultyForAnyAcademicYearQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return Ok(result);
    }

    [HttpGet("get-percentage-of-total-contributors-in-each-faculty-in-each-academic-year")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYear()
    {
        var query = new GetPercentageOfTotalContributorsInEachFacultyInEachAcademicYearQuery();

        var result = await _mediatorSender.Send(query);

        return Ok(result);
    }

    [HttpGet("get-percentage-of-total-contributors-by-each-faculty-for-any-academic-year/{AcademicYearName}")]
    [Authorize(Permissions.Dashboards.View)]
    public async Task<IActionResult> GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYear([FromRoute] GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearRequest request)
    {
        var mapper = _mapper.Map<GetPercentageOfTotalContributorsByEachFacultyForAnyAcademicYearQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return Ok(result);
    }
}
