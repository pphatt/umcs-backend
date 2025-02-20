using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.AcademicYear;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.AcademicYearsApp.Queries.GetAllAcademicYearsPagination;

public class GetAllAcademicYearsPaginationQueryHandler : IRequestHandler<GetAllAcademicYearsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<AcademicYearDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllAcademicYearsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<AcademicYearDto>>>> Handle(GetAllAcademicYearsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result =
            await _unitOfWork
                .AcademicYearRepository
                .GetAllAcademicYearsPagination(keyword: request.Keyword, pageIndex: request.PageIndex, pageSize: request.PageSize);

        return new ResponseWrapper<PaginationResult<AcademicYearDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
