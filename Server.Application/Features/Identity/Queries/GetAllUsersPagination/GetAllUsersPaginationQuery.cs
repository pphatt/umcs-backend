using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos.Identity.Users;

namespace Server.Application.Features.Identity.Queries.GetAllUsersPagination;

public class GetAllUsersPaginationQuery : IRequest<ErrorOr<List<UserDto>>>
{
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
