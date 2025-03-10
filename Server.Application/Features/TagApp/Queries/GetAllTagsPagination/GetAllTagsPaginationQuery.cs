using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Content.Tag;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.TagApp.Queries.GetAllTagsPagination;

public class GetAllTagsPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<TagInListDto>>>>
{
}
