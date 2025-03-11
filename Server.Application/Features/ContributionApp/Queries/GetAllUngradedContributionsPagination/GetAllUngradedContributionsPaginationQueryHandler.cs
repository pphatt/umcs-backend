using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.GetAllUngradedContributionsPagination;

public class GetAllUngradedContributionsPaginationQueryHandler : IRequestHandler<GetAllUngradedContributionsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<UngradedContributionDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUngradedContributionsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<UngradedContributionDto>>>> Handle(GetAllUngradedContributionsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ContributionRepository.GetAllUngradedContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            academicYearName: request.AcademicYear,
            facultyName: request.Faculty,
            orderBy: request.OrderBy
        );

        return new ResponseWrapper<PaginationResult<UngradedContributionDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
