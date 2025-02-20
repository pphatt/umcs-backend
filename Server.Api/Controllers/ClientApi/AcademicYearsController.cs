using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;
using Server.Contracts.AcademicYears.GetAllAcademicYearsPagination;

namespace Server.Api.Controllers.ClientApi;

public class AcademicYearsController : ClientApiController
{
    private readonly IMapper _mapper;

    public AcademicYearsController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetAllAcademicYearsPagination([FromQuery] GetAllAcademicYearsPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllAcademicYearsPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
