using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionActivityLogsApp.Queries.GetAllContributionActivityLogsPagination;

public class GetAllContributionActivityLogsPaginationQueryHandler : IRequestHandler<GetAllContributionActivityLogsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<ContributionActivityLogsDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllContributionActivityLogsPaginationQueryHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<ContributionActivityLogsDto>>>> Handle(
        GetAllContributionActivityLogsPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.ContributionActivityLogRepository.GetAllContributionActivityLogsPagination(
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            facultyName: request.FacultyName,
            academicYearName: request.AcademicYearName);

        return new ResponseWrapper<PaginationResult<ContributionActivityLogsDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
