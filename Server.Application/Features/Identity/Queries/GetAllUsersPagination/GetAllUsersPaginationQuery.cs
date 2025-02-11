using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.Identity.Queries.GetAllUsersPagination;

public class GetAllUsersPaginationQuery : PaginationDto, IRequest<ErrorOr<PaginationResult<UserDto>>>
{
}
