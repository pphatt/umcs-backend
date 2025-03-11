using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Contribution;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.ContributionApp.Queries.GetAllContributionsPagination;

public class GetAllContributionsPaginationQueryHandler : IRequestHandler<GetAllContributionsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllContributionsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<ContributionInListDto>>>> Handle(GetAllContributionsPaginationQuery request, CancellationToken cancellationToken)
    {
        var contributions = await _unitOfWork.ContributionRepository.GetAllContributionsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize,
            userId: request.UserId,
            facultyName: request.FacultyName,
            academicYearName: request.AcademicYearName,
            allowedGuest: request.AllowedGuest,
            status: request.Status,
            orderBy: request.OrderBy
        );

        return new ResponseWrapper<PaginationResult<ContributionInListDto>>
        {
            IsSuccessful = true,
            Message = "Get all pagination contributions successfully.",
            ResponseData = contributions
        };
    }
}
