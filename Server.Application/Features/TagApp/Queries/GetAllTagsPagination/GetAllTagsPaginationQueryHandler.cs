using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Common.Interfaces.Persistence;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.TagApp.Queries.GetAllTagsPagination;

public class GetAllTagsPaginationQueryHandler : IRequestHandler<GetAllTagsPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<TagInListDto>>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTagsPaginationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<TagInListDto>>>> Handle(GetAllTagsPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.TagRepository.GetAllTagsPagination(
            keyword: request.Keyword,
            pageIndex: request.PageIndex,
            pageSize: request.PageSize
        );

        return new ResponseWrapper<PaginationResult<TagInListDto>>
        {
            IsSuccessful = true,
            ResponseData = result
        };
    }
}
