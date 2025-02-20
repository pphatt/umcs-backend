using AutoMapper;
using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Faculty;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.FacultyApp.Queries.GetAllFacultiesPagination;

public class GetAllFacultiesPaginationQueryHandler : IRequestHandler<GetAllFacultiesPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<FacultyDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllFacultiesPaginationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<FacultyDto>>>> Handle(GetAllFacultiesPaginationQuery request, CancellationToken cancellationToken)
    {
        var faculties =
            await _unitOfWork
                .FacultyRepository
                .GetAllFacultiesPagination(keyword: request.Keyword, pageIndex: request.PageIndex, pageSize: request.PageSize);

        return new ResponseWrapper<PaginationResult<FacultyDto>>
        {
            IsSuccessful = true,
            ResponseData = faculties
        };
    }
}
