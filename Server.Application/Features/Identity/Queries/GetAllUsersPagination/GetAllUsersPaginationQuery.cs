using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Identity.Users;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.Identity.Queries.GetAllUsersPagination;

public class GetAllUsersPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<UserDto>>>>
{
    public string? FacultyName { get; set; }

    public string? RoleName { get; set; }
}
