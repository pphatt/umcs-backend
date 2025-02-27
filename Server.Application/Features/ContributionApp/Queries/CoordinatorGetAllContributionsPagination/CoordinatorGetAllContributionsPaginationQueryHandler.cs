using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.CoordinatorGetAllContributionsPagination;

public class CoordinatorGetAllContributionsPaginationQueryHandler : IRequestHandler<CoordinatorGetAllContributionsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CoordinatorGetAllContributionsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>> Handle(CoordinatorGetAllContributionsPaginationQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _unitOfWork.ContributionRepository.GetAllContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            academicYear: request.AcademicYear,
            faculty: request.Faculty,
            status: request.Status
        );

        return new ResponseWrapper<PaginationResult<ContributionInListDto>>
        {
            IsSuccessful = true,
            Message = "Get all pagination contributions successfully.",
            ResponseData = contributions
        };
    }
}
