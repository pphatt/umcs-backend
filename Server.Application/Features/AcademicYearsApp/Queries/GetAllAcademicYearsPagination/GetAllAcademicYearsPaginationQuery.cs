using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;

public class GetAllAcademicYearsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<AcademicYearDto>>>>
{
}
