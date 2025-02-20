using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Application.Common.Dtos.Identity.Role;
using Server.Application.Wrapper;
using Server.Application.Wrapper.Pagination;
using Server.Domain.Entity.Identity;

namespace Server.Application.Features.Role.Queries.GetAllRolesPagination;

public class GetAllRolesPaginationQueryHandler : IRequestHandler<GetAllRolesPaginationQuery, ErrorOr<ResponseWrapper<PaginationResult<RoleDto>>>>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;

    public GetAllRolesPaginationQueryHandler(RoleManager<AppRole> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<ErrorOr<ResponseWrapper<PaginationResult<RoleDto>>>> Handle(GetAllRolesPaginationQuery request, CancellationToken cancellationToken)
    {
        var allRolesQuery = _roleManager.Roles;

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            allRolesQuery.Where(
                r => r.Name!.Contains(request.Keyword) ||
                     r.DisplayName!.Contains(request.Keyword)
            );
        }

        var count = await allRolesQuery.CountAsync();

        var pageIndex = request.PageIndex < 0 ? 1 : request.PageIndex;
        var skipPage = (pageIndex - 1) * request.PageSize;

        allRolesQuery =
            allRolesQuery
                .Skip(skipPage)
                .Take(request.PageSize);

        var result = await _mapper.ProjectTo<RoleDto>(allRolesQuery).ToListAsync(cancellationToken);

        return new ResponseWrapper<PaginationResult<RoleDto>>
        {
            IsSuccessful = true,
            ResponseData = new PaginationResult<RoleDto>
            {
                CurrentPage = request.PageIndex,
                PageSize = request.PageSize,
                Results = result,
                RowCount = count
            }
        };
    }
}
