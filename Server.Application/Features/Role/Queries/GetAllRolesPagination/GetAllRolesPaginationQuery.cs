using ErrorOr;
using MediatR;
using Server.Application.Common.Dtos;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;

namespace Server.Application.Features.Role.Queries.GetAllRolesPagination;

public class GetAllRolesPaginationQuery : PaginationDto, IRequest<ErrorOr<ResponseWrapper<PaginationResult<RoleDto>>>>
{
}
