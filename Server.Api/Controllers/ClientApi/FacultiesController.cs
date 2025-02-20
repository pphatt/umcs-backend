using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;
using Server.Contracts.Faculties.GetAllFacultiesPagination;

namespace Server.Api.Controllers.ClientApi;

public class FacultiesController : ClientApiController
{
    private readonly IMapper _mapper;

    public FacultiesController(ISender mediatorSender, IMapper mapper) : base(mediatorSender)
    {
        _mapper = mapper;
    }

    [HttpGet("pagination")]
    public async Task<IActionResult> GetAllFacultiesPagination([FromQuery] GetAllFacultiesPaginationRequest request)
    {
        var mapper = _mapper.Map<GetAllFacultiesPaginationQuery>(request);

        var result = await _mediatorSender.Send(mapper);

        return result.Match(
            paginationResult => Ok(paginationResult),
            errors => Problem(errors)
        );
    }
}
