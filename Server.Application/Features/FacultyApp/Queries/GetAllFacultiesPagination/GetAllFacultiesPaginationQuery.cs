using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;

public class GetAllFacultiesPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<FacultyDto>>>>
{
}
